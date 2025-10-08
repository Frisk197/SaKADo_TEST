use spacetimedb::ReducerContext;
use crate::db_vector3::DbVector3;
use papaya::HashMap;

pub struct VoxelPhysics {}

impl VoxelPhysics{
    pub fn voxel_position(voxel_size: f32, position: DbVector3) -> DbVector3{
        DbVector3{
            x: f32::round(position.x/voxel_size)*voxel_size,
            y: f32::round(position.y/voxel_size)*voxel_size,
            z: f32::round(position.z/voxel_size)*voxel_size
        }
    }

    pub fn does_voxel_collide(position: DbVector3, voxel_size: f32, map: &HashMap<DbVector3, String>) -> bool{
        map.pin().contains_key(&Self::voxel_position(voxel_size, position))
    }

    /// return is_fucked: bool, hit: bool, position: DbVector3 <br>
    /// if is_fucked is true (if a DbVector3 was NaN, full of 0 or Infinite), returns finish position
    pub fn fast_voxel_traversal_step(start: DbVector3, finish: DbVector3, voxel_size: f32, map: &HashMap<DbVector3, String>) -> (bool, bool, DbVector3){
        let direction = (finish-start).normalize();
        if direction.is_fucked(){
            return (true, false, finish)
        }

        let current_voxel_position = Self::voxel_position(voxel_size, start);

        let mut crossing = DbVector3::new(
            start.x + direction.x * (voxel_size - (start.x-current_voxel_position.x).abs()),
            start.y + direction.y * (voxel_size - (start.y-current_voxel_position.y).abs()),
            start.z + direction.z * (voxel_size - (start.z-current_voxel_position.z).abs())
        );

        if crossing.is_fucked(){
            return (true, false, finish)
        }

        crossing = (crossing-start)/direction;

        let next_position = start + direction * f32::min(f32::min(crossing.x, crossing.y), crossing.z);

        if next_position.is_fucked(){
            return (true, false, finish)
        }

        if DbVector3::dot(direction, finish - next_position)<=0f32{
            return (false, false, finish)
        }
        (false, Self::does_voxel_collide(next_position, voxel_size, map), next_position)
    }

    /// returns last_step_position: DbVector3, current_step_position: DbVector3 and hit: bool
    pub fn voxel_ray_cast(start: DbVector3, finish: DbVector3, voxel_size: f32, map: &HashMap<DbVector3, String>) -> (DbVector3, DbVector3, bool){
        let mut last_position: DbVector3;
        let mut current_position: DbVector3 = start;
        let mut hit: bool;
        loop{
            last_position = current_position;
            let mut is_fucked = false;

            (is_fucked, hit, current_position) = Self::fast_voxel_traversal_step(current_position, finish, voxel_size, map);

            if is_fucked{return (finish, finish, false)}
            if current_position==finish || hit{break}
        }
        (last_position, current_position, hit)
    }

    /// returns last_step_position: DbVector3, current_step_position: DbVector3 and hit: bool
    pub fn voxel_ray_cast_through(start: DbVector3, finish: DbVector3, voxel_size: f32, map: &HashMap<DbVector3, String>) -> Vec<DbVector3>{
        let mut current_position: DbVector3 = start;
        let mut hit_positions: Vec<DbVector3> = Vec::new();
        let mut hit: bool;
        loop{
            let is_fucked: bool;

            (is_fucked, hit, current_position) = Self::fast_voxel_traversal_step(current_position, finish, voxel_size, map);

            if is_fucked{return hit_positions}

            if hit{hit_positions.insert(hit_positions.len(), Self::voxel_position(voxel_size, current_position))}

            if current_position==finish{break}
        }
        hit_positions
    }

    ///// chunk version, not used for now
    // pub fn chunk_position(chunk_size: u64, voxel_size: f32, position: DbVector3) -> DbVector3{
    //     let mut voxel_position = Self::voxel_position(voxel_size, position);
    //     (voxel_position/(chunk_size as f32*voxel_size)).floor()
    // }
    // pub fn voxel_in_chunk_position(chunk_size: u64, voxel_size: f32, position: DbVector3) -> (DbVector3, DbVector3){
    //     let voxel_position = Self::voxel_position(voxel_size, position);
    //     let chunk_pos = Self::chunk_position(chunk_size, voxel_size, voxel_position);
    //     ((voxel_position-(chunk_pos*chunk_size as f32*voxel_size))/voxel_size, chunk_pos)
    // }
    // pub fn does_voxel_collide(ctx: &ReducerContext, chunk_size: u64, voxel_size: f32, position: DbVector3) -> bool{
    //     let voxel_in_chunk_position = Self::voxel_in_chunk_position(chunk_size, voxel_size, position);
    //     let chunk = Some(ctx.db.voxel_grid().idx_position_xyz().filter((voxel_in_chunk_position.1.x as i64, voxel_in_chunk_position.1.y as i64, voxel_in_chunk_position.1.z as i64)));
    //     if chunk.is_none(){
    //         false
    //     } else {
    //         for voxel_grid in chunk.unwrap(){
    //             return *voxel_grid.chunk_data.get(voxel_in_chunk_position.0.x as usize).unwrap().get(voxel_in_chunk_position.0.y as usize).unwrap().get(voxel_in_chunk_position.0.z as usize).unwrap() == 0u32
    //         }
    //         false
    //     }
    // }
}