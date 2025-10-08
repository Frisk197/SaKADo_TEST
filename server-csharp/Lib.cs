using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using SpacetimeDB;
using System.Diagnostics;

public static partial class Module
{
    
    [SpacetimeDB.Type]
    public partial struct DbVector2(float x, float y)
    {
        public float X = x;
        public float Y = y;

        public static DbVector2 operator +(DbVector2 vector, DbVector2 value) => new DbVector2(vector.X + value.X, vector.Y + value.Y);
        public static DbVector2 operator +(DbVector2 vector, float value) => new DbVector2(vector.X + value, vector.Y + value);
        
        public static DbVector2 operator -(DbVector2 vector, DbVector2 value) => new DbVector2(vector.X - value.X, vector.Y - value.Y);
        public static DbVector2 operator -(DbVector2 vector, float value) => new DbVector2(vector.X - value, vector.Y - value);
        
        public static DbVector2 operator *(DbVector2 vector, DbVector2 value) => new DbVector2(vector.X * value.X, vector.Y * value.Y);
        public static DbVector2 operator *(DbVector2 vector, float value) => new DbVector2(vector.X * value, vector.Y * value);
        
        public static DbVector2 operator /(DbVector2 vector, DbVector2 value) => new DbVector2(vector.X / value.X, vector.Y / value.Y);
        public static DbVector2 operator /(DbVector2 vector, float value) => new DbVector2(vector.X / value, vector.Y / value);
        
        public static DbVector2 Normalize(DbVector2 value) => value / value.Length();
        
        public readonly float Length() => MathF.Sqrt(LengthSquared());
        public readonly float LengthSquared() => Dot(this, this);
        public static float Dot(DbVector2 vector1, DbVector2 vector2) =>   (vector1.X * vector2.X)
                                                                         + (vector1.Y * vector2.Y);
        
        public static float Distance(DbVector2 value1, DbVector2 value2) => MathF.Sqrt(DistanceSquared(value1, value2));
        public static float DistanceSquared(DbVector2 value1, DbVector2 value2)
        {
            DbVector2 difference = value1 - value2;
            return Dot(difference, difference);
        }


        // public override int GetHashCode()
        // {
        //     return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
        // }
    }
    
    [SpacetimeDB.Type]
    public partial struct DbVector3(float x, float y, float z)
    {
        public float X = x;
        public float Y = y;
        public float Z = z;

        public static DbVector3 operator +(DbVector3 vector, DbVector3 value) => new DbVector3(vector.X + value.X, vector.Y + value.Y, vector.Z + value.Z);
        public static DbVector3 operator +(DbVector3 vector, float value) => new DbVector3(vector.X + value, vector.Y + value, vector.Z + value);
        
        public static DbVector3 operator -(DbVector3 vector, DbVector3 value) => new DbVector3(vector.X - value.X, vector.Y - value.Y, vector.Z - value.Z);
        public static DbVector3 operator -(DbVector3 vector, float value) => new DbVector3(vector.X - value, vector.Y - value, vector.Z - value);
        
        public static DbVector3 operator *(DbVector3 vector, DbVector3 value) => new DbVector3(vector.X * value.X, vector.Y * value.Y, vector.Z * value.Z);
        public static DbVector3 operator *(DbVector3 vector, float value) => new DbVector3(vector.X * value, vector.Y * value, vector.Z * value);
        public static DbVector3 operator *(DbQuaternion rotation, DbVector3 point)
        {
            float num1 = rotation.X * 2f;
            float num2 = rotation.Y * 2f;
            float num3 = rotation.Z * 2f;
            float num4 = rotation.X * num1;
            float num5 = rotation.Y * num2;
            float num6 = rotation.Z * num3;
            float num7 = rotation.X * num2;
            float num8 = rotation.X * num3;
            float num9 = rotation.Y * num3;
            float num10 = rotation.W * num1;
            float num11 = rotation.W * num2;
            float num12 = rotation.W * num3;
            DbVector3 vector3;
            vector3.X = (float) ((1.0 - ((double) num5 + (double) num6)) * (double) point.X + ((double) num7 - (double) num12) * (double) point.Y + ((double) num8 + (double) num11) * (double) point.Z);
            vector3.Y = (float) (((double) num7 + (double) num12) * (double) point.X + (1.0 - ((double) num4 + (double) num6)) * (double) point.Y + ((double) num9 - (double) num10) * (double) point.Z);
            vector3.Z = (float) (((double) num8 - (double) num11) * (double) point.X + ((double) num9 + (double) num10) * (double) point.Y + (1.0 - ((double) num4 + (double) num5)) * (double) point.Z);
            return vector3;
        }
        
        public static DbVector3 operator /(DbVector3 vector, DbVector3 value) => new DbVector3(vector.X / value.X, vector.Y / value.Y, vector.Z / value.Z);
        public static DbVector3 operator /(DbVector3 vector, float value) => new DbVector3(vector.X / value, vector.Y / value, vector.Z / value);

        public static DbVector3 Right(DbQuaternion quaternion) => quaternion * new DbVector3(1,0,0);
        public static DbVector3 Up(DbQuaternion quaternion) => quaternion * new DbVector3(0,1,0);
        public static DbVector3 Forward(DbQuaternion quaternion) => quaternion * new DbVector3(0,0,1);
        
        public static DbVector3 Normalize(DbVector3 value) => value / value.Length();
        
        public readonly float Length() => MathF.Sqrt(LengthSquared());
        
        public readonly float LengthSquared() => Dot(this, this);
        
        public static float Dot(DbVector3 vector1, DbVector3 vector2) =>   (vector1.X * vector2.X)
                                                                         + (vector1.Y * vector2.Y)
                                                                         + (vector1.Z * vector2.Z);
        
        public static float Distance(DbVector3 value1, DbVector3 value2) => MathF.Sqrt(DistanceSquared(value1, value2));
        
        public static float DistanceSquared(DbVector3 value1, DbVector3 value2)
        {
            DbVector3 difference = value1 - value2;
            return Dot(difference, difference);
        }


        // public override int GetHashCode()
        // {
        //     return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
        // }
    }
    
    [SpacetimeDB.Type]
    public partial struct DbQuaternion(float x, float y, float z, float w)
    {
        public float X = x;
        public float Y = y;
        public float Z = z;
        public float W = w;
    }

    
    [SpacetimeDB.Type]
    public partial struct ControllerInput
    {
        public DbVector3 Position;
        public DbQuaternion Rotation;
        public bool IsAXPressed;
        public bool WasAXPressedThisFrame;
        public bool IsBYPressed;
        public bool WasBYPressedThisFrame;
        public float IndexTriggerValue;
        public bool IndexTriggerPressedThisFrame;
        public float GripTriggerValue;
        public bool GripTriggerPressedThisFrame;
        public DbVector2 JoystickPosition;
    }
    

    [SpacetimeDB.Type]
    public enum TrackerType
    {
        Head,
        RightHand,
        LeftHand
    }

    [SpacetimeDB.Table(Name = "Tracker")]
    public partial struct Tracker
    {
        [PrimaryKey, AutoInc]
        public uint TrackerId;
        public TrackerType TrackerType;
        [SpacetimeDB.Index.BTree]
        public Identity PlayerIdentity;
        public DbVector3 Position;
        public DbQuaternion Rotation;
    }

    [SpacetimeDB.Table(Name = "RaycastDebugger", Public = true)]
    public partial struct RaycastDebugger
    {
        [PrimaryKey, AutoInc] public uint Id;
        [SpacetimeDB.Index.BTree]
        public Identity PlayerIdentity;
        public TrackerType TrackerTypeAttached;
        public DbVector3 Position;
    }
    
    [SpacetimeDB.Table(Name = "Player", Public = true)]
    [SpacetimeDB.Table(Name = "LoggedOutPlayer")]
    public partial struct Player
    {
        [PrimaryKey] public Identity Identity;
        [Unique, AutoInc] public uint PlayerId;
    }
    
    [SpacetimeDB.Table(Name = "VoxelWorld")]
    public partial struct VoxelWorld
    {
        [PrimaryKey, AutoInc] public uint Id;
        public float VoxelSize;
    }

    [SpacetimeDB.Table(Name = "VoxelGrid")]
    public partial struct VoxelGrid
    {
        [PrimaryKey, AutoInc]
        public uint Id;
        [SpacetimeDB.Index.BTree] public int PositionHash;
        public DbVector3 Position;
    }


    [Reducer(ReducerKind.Init)]
    public static void Init(ReducerContext ctx)
    {
        ctx.Db.VoxelWorld.Insert(new VoxelWorld
        {
            VoxelSize = 0.1f
        });
    }
    
    
    [Reducer(ReducerKind.ClientConnected)]
    public static void Connect(ReducerContext ctx)
    {
        var player = ctx.Db.LoggedOutPlayer.Identity.Find(ctx.Sender);

        Nullable<Player> insertedPlayer = null;
        
        if (player != null)
        {
            insertedPlayer = ctx.Db.Player.Insert(player.Value);
            ctx.Db.LoggedOutPlayer.Identity.Delete(player.Value.Identity);
        }
        else
        {
            insertedPlayer = ctx.Db.Player.Insert(new Player
            {
                Identity = ctx.Sender
            });
        }
        
        ctx.Db.Tracker.Insert(new Tracker
        {
            PlayerIdentity = insertedPlayer.Value.Identity,
            TrackerType = TrackerType.Head,
            Position = new DbVector3(0, 0, 0),
            Rotation = new DbQuaternion()
        });
        ctx.Db.Tracker.Insert(new Tracker
        {
            PlayerIdentity = insertedPlayer.Value.Identity,
            TrackerType = TrackerType.RightHand,
            Position = new DbVector3(0, 0, 0),
            Rotation = new DbQuaternion()
        });
        ctx.Db.Tracker.Insert(new Tracker
        {
            PlayerIdentity = insertedPlayer.Value.Identity,
            TrackerType = TrackerType.LeftHand,
            Position = new DbVector3(0, 0, 0),
            Rotation = new DbQuaternion()
        });
        
        ctx.Db.RaycastDebugger.Insert(new RaycastDebugger
        {
            PlayerIdentity = insertedPlayer.Value.Identity,
            TrackerTypeAttached = TrackerType.RightHand,
            Position = new DbVector3(0, 0, 0),
        });
        ctx.Db.RaycastDebugger.Insert(new RaycastDebugger
        {
            PlayerIdentity = insertedPlayer.Value.Identity,
            TrackerTypeAttached = TrackerType.LeftHand,
            Position = new DbVector3(0, 0, 0),
        });
    }

    [Reducer(ReducerKind.ClientDisconnected)]
    public static void Disconnect(ReducerContext ctx)
    {
        var player = ctx.Db.Player.Identity.Find(ctx.Sender) ?? throw new Exception("Player '" + ctx.Sender + "' not found.");
        var trackers = ctx.Db.Tracker.PlayerIdentity.Filter(ctx.Sender);
        foreach (var tracker in trackers)
        {
            ctx.Db.Tracker.TrackerId.Delete(tracker.TrackerId);
        }
        var rayCastDebuggers = ctx.Db.RaycastDebugger.PlayerIdentity.Filter(ctx.Sender);
        foreach (var rayCastDebugger in rayCastDebuggers)
        {
            ctx.Db.RaycastDebugger.Id.Delete(rayCastDebugger.Id);
        }
        ctx.Db.LoggedOutPlayer.Insert(player);
        ctx.Db.Player.Identity.Delete(player.Identity);
    }

    [Reducer]
    public static void UpdatePlayer(ReducerContext ctx, DbVector3 headPosition, DbQuaternion headRotation, ControllerInput leftController, ControllerInput rightController)
    {
        var voxelSize = (ctx.Db.VoxelWorld.Id.Find(1) ?? throw new Exception("World 1 doesn't exist.")).VoxelSize;
        var trackers = ctx.Db.Tracker.PlayerIdentity.Filter(ctx.Sender);
        var rayCastDebuggers = ctx.Db.RaycastDebugger.PlayerIdentity.Filter(ctx.Sender);

        DbVector3 leftRayCastDebug = new DbVector3();
        DbVector3 rightRayCastDebug = new DbVector3();
        
        foreach (var tracker in trackers)
        {
            var newTracker = tracker;
            switch (tracker.TrackerType)
            {
                case TrackerType.Head:
                    newTracker.Position = headPosition;
                    newTracker.Rotation = headRotation;
                    ctx.Db.Tracker.TrackerId.Update(newTracker);
                    break;
                case TrackerType.LeftHand:
                    newTracker.Position = leftController.Position;
                    newTracker.Rotation = leftController.Rotation;
                    if (leftController.IsAXPressed)
                    {
                        var rayCast = VoxelPhysics.VoxelRayCast(ref ctx, voxelSize, leftController.Position,
                            leftController.Position + DbVector3.Forward(leftController.Rotation) * 3);
                        leftRayCastDebug = rayCast.hitPosition;
                    }
                    ctx.Db.Tracker.TrackerId.Update(newTracker);
                    break;
                case TrackerType.RightHand:
                    newTracker.Position = rightController.Position;
                    newTracker.Rotation = rightController.Rotation;
                    if (rightController.IsAXPressed)
                    {
                        var rayCast = VoxelPhysics.VoxelRayCast(ref ctx, voxelSize, rightController.Position,
                            rightController.Position + DbVector3.Forward(rightController.Rotation) * 3);
                        rightRayCastDebug = rayCast.hitPosition;
                    }
                    ctx.Db.Tracker.TrackerId.Update(newTracker);
                    break;
                default:
                    break;
            }
        }

        foreach (var rayCastDebugger in rayCastDebuggers)
        {
            var newRayCastDebugger = rayCastDebugger;
            switch (rayCastDebugger.TrackerTypeAttached)
            {
                case TrackerType.LeftHand:
                    newRayCastDebugger.Position = leftRayCastDebug;
                    break;
                case TrackerType.RightHand:
                    newRayCastDebugger.Position = rightRayCastDebug;
                    break;
                default:
                    break;
            }

            ctx.Db.RaycastDebugger.Id.Update(newRayCastDebugger);
        }
    }

    [Reducer]
    public static void SendPointsToServer(ReducerContext ctx, DbVector3 origin, DbVector3[] points)
    {
        
        // Log.Debug("hello");
        
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        
        var voxelSize = (ctx.Db.VoxelWorld.Id.Find(1) ?? throw new Exception("World 1 doesn't exist.")).VoxelSize;

        int countVoxelCheck = 0;
        int countVoxelInsert = 0;
        int countVoxelDelete = 0;
        bool isFucked = false;
        
        foreach (var point in points)
        {
            (List<DbVector3> hitsPositions, int voxelCheck, bool isFuckedHere) = VoxelPhysics.VoxelRayCastThrough(ctx, voxelSize, origin, point);
            if (!isFucked && isFuckedHere)
                isFucked = true;
            countVoxelCheck += voxelCheck;

            foreach (var hit in hitsPositions)
                ctx.Db.VoxelGrid.PositionHash.Delete(VoxelPhysics.RawPositionToVoxelPosition(voxelSize, hit).GetHashCode());

            countVoxelDelete += hitsPositions.Count;

            var voxel = VoxelPhysics.RawPositionToVoxelPosition(voxelSize, point);
            
            if (!ctx.Db.VoxelGrid.PositionHash.Filter(voxel.GetHashCode()).Any())
            {
                ++countVoxelInsert;
                ctx.Db.VoxelGrid.Insert(new VoxelGrid
                {
                    Position = voxel,
                    PositionHash = voxel.GetHashCode()
                });
            }
        }
        
        stopWatch.Stop();
        if(stopWatch.ElapsedMilliseconds > 10)
            Log.Debug("finished " + points.Length + " point in " + stopWatch.ElapsedMilliseconds + " with " + countVoxelCheck + " checks, " + countVoxelInsert + " inserts and " + countVoxelDelete + " deletes." + (isFucked?" and it's fucked.":""));
    }



    public struct VoxelPhysics
    {
        public static DbVector3 RawPositionToVoxelPosition(float voxelSize, DbVector3 position)
        {
            var x = (float)Math.Round(position.X / voxelSize) * voxelSize;
            var y = (float)Math.Round(position.Y / voxelSize) * voxelSize;
            var z = (float)Math.Round(position.Z / voxelSize) * voxelSize;
            return new DbVector3(x,y,z);
        }
        
        public static bool DoesVoxelCollide(ref ReducerContext ctx, float voxelSize, DbVector3 position)
        {
            return ctx.Db.VoxelGrid.PositionHash.Filter(RawPositionToVoxelPosition(voxelSize, position).GetHashCode()).Any();
        }

        public static (bool outputIsFucked, DbVector3 stepPosition,  bool hit) FastVoxelTraversalStep(ref ReducerContext ctx, float voxelSize, DbVector3 start, DbVector3 finish)
        {
            // var stopWatch = new Stopwatch();
            // stopWatch.Start();
            DbVector3 direction = DbVector3.Normalize(finish - start);

            if (direction.X == 0 || direction.Y == 0 || direction.Z == 0 ||
                float.IsNaN(direction.X) || float.IsNaN(direction.Y) || float.IsNaN(direction.Z) ||
                float.IsInfinity(direction.X) || float.IsInfinity(direction.Y) || float.IsInfinity(direction.Z))
            {
                // Log.Info("Direction is 0, NaN or Infinity");
                return (true, finish, false);
            }
            
            DbVector3 currentVoxelPosition = RawPositionToVoxelPosition(voxelSize, start);

            DbVector3 crossing = new DbVector3(
                start.X + direction.X * (voxelSize - Math.Abs(start.X - currentVoxelPosition.X)),
                start.Y + direction.Y * (voxelSize - Math.Abs(start.Y - currentVoxelPosition.Y)),
                start.Z + direction.Z * (voxelSize - Math.Abs(start.Z - currentVoxelPosition.Z))
            );

            if (crossing.X == 0 || crossing.Y == 0 || crossing.Z == 0 ||
                float.IsNaN(crossing.X) || float.IsNaN(crossing.Y) || float.IsNaN(crossing.Z) ||
                float.IsInfinity(crossing.X) || float.IsInfinity(crossing.Y) || float.IsInfinity(crossing.Z))
            {
                // Log.Info("Crossing 1 is 0, NaN or Infinity");
                return (true, finish, false);
            }

            crossing = (crossing - start) / direction;

            if (crossing.X == 0 || crossing.Y == 0 || crossing.Z == 0 ||
                float.IsNaN(crossing.X) || float.IsNaN(crossing.Y) || float.IsNaN(crossing.Z) ||
                float.IsInfinity(crossing.X) || float.IsInfinity(crossing.Y) || float.IsInfinity(crossing.Z))
            {
                // Log.Info("Crossing 2 is 0, NaN or Infinity");
                return (true, finish, false);
            }

            var nextPosition = start + direction * Math.Min(Math.Min(crossing.X, crossing.Y), crossing.Z);

            if (nextPosition.X == 0 || nextPosition.Y == 0 || nextPosition.Z == 0 ||
                float.IsNaN(nextPosition.X) || float.IsNaN(nextPosition.Y) || float.IsNaN(nextPosition.Z) ||
                float.IsInfinity(nextPosition.X) || float.IsInfinity(nextPosition.Y) ||
                float.IsInfinity(nextPosition.Z))
            {
                // Log.Info("NextPosition is 0, NaN or Infinity");
                return (true, finish, false);
            }
            
            if (DbVector3.Dot(direction, finish - nextPosition) <= 0) return (false, finish, false);
            
            return (false, nextPosition, DoesVoxelCollide(ref ctx, voxelSize, nextPosition));
        }

        public static (DbVector3 lastStep, DbVector3 hitPosition,  bool hit) VoxelRayCast(ref ReducerContext ctx, float voxelSize, DbVector3 start, DbVector3 finish)
        {
            DbVector3 lastPosition;
            DbVector3 currentPosition = start;
            bool hit;
            do
            {
                lastPosition = currentPosition;
                (bool isFucked, currentPosition, hit) = FastVoxelTraversalStep(ref ctx, voxelSize, currentPosition, finish);
                if(isFucked)
                    return (finish, finish, false);

            } while (currentPosition != finish && !hit);
            return (lastPosition, currentPosition, hit);
        }
        
        public static (List<DbVector3>, int, bool) VoxelRayCastThrough(ReducerContext ctx, float voxelSize, DbVector3 start, DbVector3 finish)
        {
            DbVector3 currentPosition = start;
            List<DbVector3> hitsPositions = new List<DbVector3>();
            
            int i = 0;
            do
            {
                ++i;
                (bool isFucked, currentPosition, bool hit) = FastVoxelTraversalStep(ref ctx, voxelSize, currentPosition, finish);
                if (isFucked)
                    return (hitsPositions, i, isFucked);
                if (hit)
                    hitsPositions.Add(currentPosition);
                
            } while (currentPosition != finish);
            return (hitsPositions, i, false);
        }
        
    }
    
    
}
