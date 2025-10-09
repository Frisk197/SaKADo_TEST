use spacetimedb::{table, Identity, SpacetimeType};
use crate::db_quaternion::DbQuaternion;
use crate::db_vector3::DbVector3;
use crate::other_types::TrackerType;

#[table(name = tracker, index(name=idx_player_identity, btree(columns=[identity])))]
#[derive(Debug, Clone)]
pub struct Tracker{
    #[primary_key]
    #[auto_inc]
    pub tracker_id: u64,
    pub tracker_type: TrackerType,
    pub identity: Identity,
    pub position: DbVector3,
    pub rotation: DbQuaternion,
}

#[table(name=raycast_debugger, public, index(name=idx_player_identity, btree(columns=[identity])))]
#[derive(Debug, Clone)]
pub struct RaycastDebugger{
    #[primary_key]
    #[auto_inc]
    pub id: u64,
    pub identity: Identity,
    pub tracker_type_attached: TrackerType,
    pub position: DbVector3
}

#[table(name=player, public)]
#[table(name=logged_out_player)]
#[derive(Debug, Clone)]
pub struct Player{
    #[primary_key]
    pub identity: Identity,
    #[unique]
    #[auto_inc]
    pub player_id: u64
}

#[table(name=voxel_world)]
#[derive(Debug, Clone)]
pub struct VoxelWorld{
    #[primary_key]
    #[auto_inc]
    pub world_id: u64,
    // pub chunk_size: u64,
    pub voxel_size: f32,
}

#[table(name=voxel_grid, index(name=idx_position_xyz, btree(columns=[x, y, z])))]
#[derive(Debug, Clone)]
pub struct VoxelGrid{
    #[primary_key]
    #[auto_inc]
    pub id: u64,
    pub x: i64,
    pub y: i64,
    pub z: i64,
    pub chunk_data: Vec<Vec<Vec<u32>>>
}