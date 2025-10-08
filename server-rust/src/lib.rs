mod db_vector2;
mod db_vector3;
mod db_quaternion;
mod other_types;
mod tables;
mod voxel_physics;

use lazy_static::lazy_static;
use papaya::HashMap;
use spacetimedb::{ReducerContext, Table, SpacetimeType, reducer};
use db_vector2::DbVector2;
use db_vector3::DbVector3;
use crate::db_quaternion::DbQuaternion;
use crate::other_types::{ControllerInput, TrackerType};
use crate::tables::{logged_out_player, player, raycast_debugger, tracker, voxel_world, Player, RaycastDebugger, Tracker, VoxelWorld};
use crate::voxel_physics::VoxelPhysics;

static LOOP_LIMIT: usize = 300;

lazy_static!{
    static ref WORLD_GRID: HashMap<DbVector3, String> = HashMap::new();
}


#[reducer]
pub fn test(ctx: &ReducerContext) {
    let mut a = DbVector2::new(0f32, 0f32);
    a += a+1f32;
    let test = a.normalize();
}

#[reducer(init)]
pub fn init(ctx: &ReducerContext){
    ctx.db.voxel_world().insert(VoxelWorld{world_id: 0, chunk_size: 10, voxel_size: 0.1f32});
    WORLD_GRID.pin().clear()
}

#[reducer(client_connected)]
pub fn connect(ctx: &ReducerContext){
    if let Some(player) = ctx.db.logged_out_player().identity().find(&ctx.sender){
        ctx.db.player().insert(player.clone());
        ctx.db.logged_out_player().identity().delete(&ctx.sender);
    } else {
        ctx.db.player().insert(Player{identity: ctx.sender, player_id:0});
    }
    ctx.db.tracker().insert(Tracker{
        tracker_id: 0,
        tracker_type: TrackerType::Head,
        identity: ctx.sender,
        position: DbVector3::new(0f32,0f32,0f32),
        rotation: DbQuaternion::new(0f32, 0f32, 0f32, 0f32)
    });
    ctx.db.tracker().insert(Tracker{
        tracker_id: 0,
        tracker_type: TrackerType::RightHand,
        identity: ctx.sender,
        position: DbVector3::new(0f32,0f32,0f32),
        rotation: DbQuaternion::new(0f32, 0f32, 0f32, 0f32)
    });
    ctx.db.tracker().insert(Tracker{
        tracker_id: 0,
        tracker_type: TrackerType::LeftHand,
        identity: ctx.sender,
        position: DbVector3::new(0f32,0f32,0f32),
        rotation: DbQuaternion::new(0f32, 0f32, 0f32, 0f32)
    });

    ctx.db.raycast_debugger().insert(RaycastDebugger{
        id: 0,
        tracker_type_attached: TrackerType::RightHand,
        identity: ctx.sender,
        position: DbVector3::new(0f32,0f32,0f32),
    });
    ctx.db.raycast_debugger().insert(RaycastDebugger{
        id: 0,
        tracker_type_attached: TrackerType::LeftHand,
        identity: ctx.sender,
        position: DbVector3::new(0f32,0f32,0f32),
    });
}

#[reducer(client_disconnected)]
pub fn disconnect(ctx: &ReducerContext){
    if let Some(player) = ctx.db.player().identity().find(&ctx.sender){
        ctx.db.logged_out_player().insert(player.clone());
        ctx.db.player().identity().delete(&ctx.sender);
        ctx.db.tracker().idx_player_identity().delete(&ctx.sender);
        ctx.db.raycast_debugger().idx_player_identity().delete(&ctx.sender);
    }
}

#[reducer]
pub fn update_player(ctx: &ReducerContext, head_position: DbVector3, head_rotation: DbQuaternion, left_controller: ControllerInput, right_controller: ControllerInput){
    let world_size = ctx.db.voxel_world().world_id().find(1).ok_or("world 1 not found").unwrap().voxel_size;
    let trackers = ctx.db.tracker().idx_player_identity().filter(ctx.sender);
    let raycast_debuggers = ctx.db.raycast_debugger().idx_player_identity().filter(ctx.sender);

    let mut left_raycast_debug = DbVector3::new(0f32, 0f32, 0f32);
    let mut right_raycast_debug = DbVector3::new(0f32, 0f32, 0f32);

    for tracker in trackers {
        let mut new_tracker = tracker;
        match new_tracker.tracker_type{
            TrackerType::Head =>{
                new_tracker.position = head_position;
                new_tracker.rotation = head_rotation;
                ctx.db.tracker().tracker_id().update(new_tracker);
            },
            TrackerType::LeftHand =>{
                left_raycast_debug = left_controller.position;
                new_tracker.position = left_controller.position;
                new_tracker.rotation = left_controller.rotation;
                if left_controller.is_ax_pressed{
                    let (last_step_position, current_step_position, hit) = VoxelPhysics::voxel_ray_cast(left_controller.position, left_controller.position + DbVector3::forward(left_controller.rotation)*3f32, world_size, &WORLD_GRID);
                    if hit{
                        left_raycast_debug = current_step_position;
                    }
                }
                ctx.db.tracker().tracker_id().update(new_tracker);
            },
            TrackerType::RightHand =>{
                right_raycast_debug = right_controller.position;
                new_tracker.position = right_controller.position;
                new_tracker.rotation = right_controller.rotation;
                if right_controller.is_ax_pressed{
                    let (last_step_position, current_step_position, hit) = VoxelPhysics::voxel_ray_cast(right_controller.position, right_controller.position + DbVector3::forward(right_controller.rotation)*3f32, world_size, &WORLD_GRID);
                    if hit{
                        right_raycast_debug = current_step_position;
                    }
                }
                ctx.db.tracker().tracker_id().update(new_tracker);
            },
            _=>{},
        }
    }

    for raycast_debugger in raycast_debuggers {
        let mut new_debugger = raycast_debugger;
        match new_debugger.tracker_type_attached {
            TrackerType::RightHand =>{
                new_debugger.position = right_raycast_debug;
            },
            TrackerType::LeftHand => {
                new_debugger.position = left_raycast_debug;
            },
            _=>{},
        }
        ctx.db.raycast_debugger().id().update(new_debugger);
    }
}

#[reducer]
pub fn send_points_to_server(ctx: &ReducerContext, origin: DbVector3, points: Vec<DbVector3>, call_num: i32){
    let world_size = ctx.db.voxel_world().world_id().find(1).ok_or("world 1 not found").unwrap().voxel_size;

    // for point in points{
    //     let hit_points = VoxelPhysics::voxel_ray_cast_through(origin, point, world_size, &WORLD_GRID);
    //     for hit in hit_points{
    //         map.remove(&hit);
    //     }
    //     let voxel_position = VoxelPhysics::voxel_position(world_size, point);
    //     if !map.contains_key(&voxel_position){
    //         map.insert(voxel_position, "voxel".parse().unwrap());
    //     }
    // }

    let l = points.len();
    let loop_iter = f32::ceil(l as f32/LOOP_LIMIT as f32) as usize;

    rayon::scope(|s|{
        for i in 0..loop_iter {
            let p = points.clone();
            s.spawn(move |_| {
                let points = p;
                for j in (LOOP_LIMIT*i)..usize::min(LOOP_LIMIT*i+LOOP_LIMIT, l){
                    let hit_points = VoxelPhysics::voxel_ray_cast_through(origin, points[j], world_size, &WORLD_GRID);
                    let map = WORLD_GRID.pin();
                    for hit in hit_points{
                        map.remove(&hit);
                    }
                    let voxel_position = VoxelPhysics::voxel_position(world_size, points[j]);
                    if !map.contains_key(&voxel_position){
                        map.insert(voxel_position, "voxel".parse().unwrap());
                    }
                }
                // log::info!("finished call {call_num}, {i}")
            });
        }
    });
}

#[reducer]
pub fn debug_map_size(ctx: &ReducerContext){
    let l = WORLD_GRID.len();
    log::debug!("map is length: {l}");
}





// #[spacetimedb::table(name = person)]
// pub struct Person {
//     name: String
// }
//
// #[spacetimedb::reducer(init)]
// pub fn init(_ctx: &ReducerContext) {
//     // Called when the module is initially published
// }
//
// #[spacetimedb::reducer(client_connected)]
// pub fn identity_connected(_ctx: &ReducerContext) {
//     // Called everytime a new client connects
// }
//
// #[spacetimedb::reducer(client_disconnected)]
// pub fn identity_disconnected(_ctx: &ReducerContext) {
//     // Called everytime a client disconnects
// }
//
// #[spacetimedb::reducer]
// pub fn add(ctx: &ReducerContext, name: String) {
//     ctx.db.person().insert(Person { name });
// }
//
// #[spacetimedb::reducer]
// pub fn say_hello(ctx: &ReducerContext) {
//     for person in ctx.db.person().iter() {
//         log::info!("Hello, {}!", person.name);
//     }
//     log::info!("Hello, World!");
// }
