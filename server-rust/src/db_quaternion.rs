use spacetimedb::SpacetimeType;

#[derive(SpacetimeType, Copy, Clone, Debug)]
pub struct DbQuaternion {
    pub x: f32,
    pub y: f32,
    pub z: f32,
    pub w: f32,
}

impl DbQuaternion{
    pub fn new(x: f32, y: f32, z: f32, w: f32,) -> DbQuaternion{
        DbQuaternion{ x, y, z, w }
    }
}