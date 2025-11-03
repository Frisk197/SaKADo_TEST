using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Anaglyph.XRTemplate.DepthKit;
using Meta.XR.EnvironmentDepth;
using SpacetimeDB;
using SpacetimeDB.Types;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using Unity.XR.Oculus;
using UnityEngine;
using Random = UnityEngine.Random;

public class DepthPointCloudCast : MonoBehaviour
{
    public static readonly int Meta_EnvironmentDepthTexture_ID = Shader.PropertyToID("_EnvironmentDepthTexture");
    public static readonly int Meta_ReprojectionMatrices_ID = Shader.PropertyToID("_EnvironmentDepthReprojectionMatrices");
    private static readonly int RaycastResultsId = Shader.PropertyToID("RaycastResults");
    public static readonly int dpcc_DepthTexture_ID = Shader.PropertyToID("dpcc_DepthTexture");
    private static readonly int DepthcastResultsId = Shader.PropertyToID("DepthcastResults");
    private static readonly int SampleSizeId = Shader.PropertyToID("sampleSize");
    private static readonly int NumSamples = Shader.PropertyToID("NumSamples");
    private static readonly int ShaderEye = Shader.PropertyToID("Eye");
    private int eye = 0;
    public int sampleSize = 512;
    public int pointsSentPerFrame = 10;
    public bool useYieldMethod = true;
    public Matrix4x4[] dpcc_Proj = new Matrix4x4[2];
    private ComputeBuffer computeBuffer;
    [SerializeField] private ComputeShader computeShader;
    public DepthKitDriver DepthKitDriver;

    private int sendCount = 1;
    
    private GameManager gameManager;

    private Task task;
    private IEnumerator<bool> task2;

    public Transform headPosition;
    private Vector3 lastCapturePosition;

    private float lastPlayerUpdate = 0;
    public float playerUpdateRate = 60; // in FPD

    private float lastPointCloudSend = 0;
    public float pointCloudUpdateRate = 50;


    public GameObject sphere;
    public List<GameObject> spheres;


    public bool canExecSendPoints = false;
    public bool canExecUpdatePlayer = false;
    public bool dbSetup = false;


    private void Start()
    {
        spheres = new List<GameObject>();

        for (int i = 0; i < 2; i++)
        {
            var e = Instantiate(sphere);
            e.transform.position = new Vector3(0, 0, 0);
            e.transform.rotation = Quaternion.identity;
            e.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            spheres.Add(e);
        }
        
        lastPlayerUpdate = Time.time;
        gameManager = GameManager.Instance;
        gameManager.Connect();
        task = Task.CompletedTask;
        task2 = new NoAllocEnumerator<bool>();
        computeBuffer = GetComputeBuffers(sampleSize*sampleSize);
    }

    private void Update()
    {

        if (!dbSetup)
        {
            if(!GameManager.IsConnected())
                return;
            GameManager.Conn.Db.FunctionReadyCounter.OnUpdate += updateFunctionReadyCounter;
            GameManager.Conn.Db.FunctionReadyCounter.OnInsert += insertFunctionReadyCounter;
            dbSetup = true;
        }
        
        
        UpdatePlayer();
        
        if (useYieldMethod)
        {
            if (!task2.Current)
            {
                var results = AnaglyphScan();
                task2 = AsyncSendToSTDB(results);
                task2.MoveNext();
            }
            else
            {
                task2.MoveNext();
            }
        }
        else
        {
            if (task.IsCompleted)
            {
                var results = AnaglyphScan();
                task = new Task(() => ThreadSendToSTDB(results));
            
                task.Start();
        
            }
        }
    }
    
    public void insertFunctionReadyCounter(EventContext ctx, FunctionReadyCounter functionReadyCounter)
    {
        if (functionReadyCounter.Player == GameManager.LocalIdentity)
        {
            switch (functionReadyCounter.Function)
            {
                case FunctionsCounter.SendPointsToServer:
                    canExecSendPoints = true;
                    break;
                case FunctionsCounter.UpdatePlayer:
                    canExecUpdatePlayer = true;
                    break;
            }
        }
    }

    public void updateFunctionReadyCounter(EventContext ctx, FunctionReadyCounter functionReadyCounterA, FunctionReadyCounter functionReadyCounterB)
    {
        if (functionReadyCounterA.Player == GameManager.LocalIdentity)
        {
            switch (functionReadyCounterA.Function)
            {
                case FunctionsCounter.SendPointsToServer:
                    canExecSendPoints = true;
                    break;
                case FunctionsCounter.UpdatePlayer:
                    canExecUpdatePlayer = true;
                    break;
            }
        }
    }

    public void UpdatePlayer()
    {
        if (!GameManager.IsConnected())
        {
            Debug.Log("not connected");
            return;
        }
        if (canExecUpdatePlayer)
        {
            canExecUpdatePlayer = false;
            lastPlayerUpdate = Time.time;
            

            var raycastDebuggers = GameManager.Conn.Db.RaycastDebugger.IdxPlayerIdentity.Filter(GameManager.LocalIdentity);

            foreach (var raycastDebugger in raycastDebuggers)
            {
                switch (raycastDebugger.TrackerTypeAttached)
                {
                    case TrackerType.LeftHand:
                        // Debug.Log((Vector3)tracker.DebugPosition);
                        spheres[0].transform.position = raycastDebugger.Position;
                        break;
                    case TrackerType.RightHand:
                        // Debug.Log((Vector3)tracker.DebugPosition);
                        spheres[1].transform.position = raycastDebugger.Position;
                        break;
                    default:
                        break;
                }
            }

            var theLeft = new ControllerInput
            {
                GripTriggerPressedThisFrame = OVRInput.GetDown(OVRInput.RawButton.LHandTrigger),
                GripTriggerValue = OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger),
                IndexTriggerPressedThisFrame = OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger),
                IndexTriggerValue = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger),
                IsAxPressed = OVRInput.Get(OVRInput.RawButton.X),
                IsByPressed = OVRInput.Get(OVRInput.RawButton.Y),
                WasAxPressedThisFrame = true,//OVRInput.GetDown(OVRInput.RawButton.X),
                WasByPressedThisFrame = OVRInput.GetDown(OVRInput.RawButton.Y),
                JoystickPosition = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick),
                Position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch),
                Rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch)
            };
            var theRight = new ControllerInput
            {
                GripTriggerPressedThisFrame = OVRInput.GetDown(OVRInput.RawButton.RHandTrigger),
                GripTriggerValue = OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger),
                IndexTriggerPressedThisFrame = OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger),
                IndexTriggerValue = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger),
                IsAxPressed = true,//OVRInput.Get(OVRInput.RawButton.A),
                IsByPressed = OVRInput.Get(OVRInput.RawButton.B),
                WasAxPressedThisFrame = OVRInput.GetDown(OVRInput.RawButton.A),
                WasByPressedThisFrame = OVRInput.GetDown(OVRInput.RawButton.B),
                JoystickPosition = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick),
                Position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch),
                Rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch)
            };
            
            GameManager.Conn.Reducers.UpdatePlayer(transform.position, transform.rotation, theLeft, theRight);
            
            // Debug.Log(("done update"));
        }
    }

    public Vector3[] AnaglyphScan()
    {
        
        return new Vector3[sampleSize*sampleSize];
        
        if (!DepthKitDriver.DepthAvailable)
        {
            if (Debug.isDebugBuild)
                Debug.Log("Depth incapable or disabled!");

            return Array.Empty<Vector3>();
        }
        
        DepthKitDriver.UpdateCurrentRenderingState(eye);
        
        computeShader.SetInt(NumSamples, sampleSize);
        computeShader.SetInt(ShaderEye, (int)eye);

        eye = eye == 0 ? 1 : 0;
        
        computeShader.SetBuffer(0, RaycastResultsId, computeBuffer);
        
        computeShader.GetKernelThreadGroupSizes(0, out uint kx, out uint ky, out uint kz);
        
        computeShader.Dispatch(0, Mathf.CeilToInt(sampleSize/(float)kx), Mathf.CeilToInt(sampleSize/(float)ky), 1);
        
        var results = new Vector3[sampleSize*sampleSize];
        computeBuffer.GetData(results);

        lastCapturePosition = headPosition.position;

        return results;
    }

    public Vector3[] CustomScan()
    {
        
        Shader.SetGlobalTexture(dpcc_DepthTexture_ID, Shader.GetGlobalTexture(Meta_EnvironmentDepthTexture_ID));
        
        dpcc_Proj = Shader.GetGlobalMatrixArray("_EnvironmentDepthReprojectionMatrices");
        Shader.SetGlobalMatrix(Shader.PropertyToID("dpcc_Proj_Inv"), DepthKitDriver.dk_InvProj);
        
        computeShader.SetInt(SampleSizeId, sampleSize);
        
        computeShader.SetBuffer(0, DepthcastResultsId, computeBuffer);
        
        computeShader.GetKernelThreadGroupSizes(0, out uint kx, out uint ky, out uint kz);
        
        computeShader.Dispatch(0,  Mathf.CeilToInt(sampleSize/(float)kx), Mathf.CeilToInt(sampleSize/(float)ky), 1);
        
        var results = new Vector3[sampleSize*sampleSize];
        computeBuffer.GetData(results);

        lastCapturePosition = transform.position;
        
        return results;
    }


    public void ThreadSendToSTDB(Vector3[] results)
    {
        var dbPoints = new List<DbVector3>();
        var size = results.Length;
        for (int i = 0; i < size; i++)
        {
            dbPoints.Add(new DbVector3(results[i].x, results[i].y, results[i].z));
        }
        GameManager.Conn.Reducers.SendPointsToServer(lastCapturePosition, dbPoints);
    }

    public IEnumerator<bool> AsyncSendToSTDB(Vector3[] results)
    {
        if (GameManager.IsConnected() && canExecSendPoints)
        {
            canExecSendPoints = false;
            int size = results.Length;
            for (int i = 0; i < math.ceil(size / (double)pointsSentPerFrame); i++)
            {
                var dbPoints = new List<DbVector3>();
                for (int j = 0; j < pointsSentPerFrame; j++)
                {
                    if(i*j<size)
                        dbPoints.Add(new DbVector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)));
                }

                while (Time.time < lastPointCloudSend+1/pointCloudUpdateRate)
                {
                    yield return true;
                }

                lastPointCloudSend = Time.time;
                GameManager.Conn.Reducers.SendPointsToServer(new DbVector3(0,0,0), dbPoints);
                yield return true;
                
            }
            Debug.Log("done");
        }
        else
        {
            // Debug.Log("not connected");
        }
        yield return false;
    }
    
    
    private ComputeBuffer GetComputeBuffers(int size)
    {
        return new ComputeBuffer(size, Marshal.SizeOf<Vector3>(),
            ComputeBufferType.Structured);
    }
}