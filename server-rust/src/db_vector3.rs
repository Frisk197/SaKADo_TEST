use std::fmt::Formatter;
use std::hash::{Hash, Hasher};
use std::ops;
use spacetimedb::SpacetimeType;
use crate::db_quaternion::DbQuaternion;

#[derive(SpacetimeType, Copy, Clone, Debug, PartialEq)]
pub struct DbVector3 {
    pub x: f32,
    pub y: f32,
    pub z: f32,
}


impl std::fmt::Display for DbVector3{
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "x: {}| y: {}| z: {}", self.x, self.y, self.z)
    }
}

impl Hash for DbVector3{
    fn hash<H: Hasher>(&self, state: &mut H) {
        state.write_u32(f32::to_bits(self.x) ^ f32::to_bits(self.y) << 2 ^ f32::to_bits(self.z) >> 2);
        let _ = state.finish();
    }
}

impl Eq  for DbVector3{}

impl DbVector3{
    pub fn new(x:f32, y:f32, z:f32) -> DbVector3{
        DbVector3{x, y, z}
    }
    pub fn normalize(self) ->DbVector3{
        let length = self.length();
        if length==0f32{
            DbVector3::new(0f32,0f32,0f32)
        } else {
            self/length
        }
    }
    pub fn length(self) -> f32{
        f32::sqrt(self.length_squared())
    }
    pub fn length_squared(self) -> f32{
        Self::dot(self, self)
    }
    pub fn dot(vector1: DbVector3, vector2: DbVector3) -> f32{
        (vector1.x * vector2.x) + (vector1.y * vector2.y) + (vector1.z * vector2.z)
    }
    pub fn distance(vector1: DbVector3, vector2: DbVector3) -> f32{
        f32::sqrt(Self::distance_squared(vector1, vector2))
    }
    pub fn distance_squared(vector1: DbVector3, vector2: DbVector3) -> f32{
        let diff = vector1 - vector2;
        DbVector3::dot(diff, diff)
    }
    pub fn right(quaternion: DbQuaternion) -> DbVector3{
        Self::new(1f32, 0f32, 0f32) * quaternion
    }
    pub fn up(quaternion: DbQuaternion) -> DbVector3{
        Self::new(0f32, 1f32, 0f32) * quaternion
    }
    pub fn forward(quaternion: DbQuaternion) -> DbVector3{
        Self::new(0f32, 0f32, 1f32) * quaternion
    }
    pub fn floor(self) -> DbVector3{
        DbVector3{x: self.x.floor(), y: self.y.floor(), z: self.z.floor()}
    }
    pub fn is_fucked(self) -> bool{
        self.x.is_nan() || self.y.is_nan() || self.z.is_nan() ||
            self.x.is_infinite() || self.y.is_infinite() || self.z.is_infinite() ||
            (self.x==0f32 && self.y==0f32 && self.z==0f32)
    }
}

impl ops::Add<DbVector3> for DbVector3{
    type Output = Self;
    fn add(self, rhs: DbVector3) -> Self::Output {
        Self::Output {x: self.x+rhs.x, y: self.y+rhs.y, z: self.z+rhs.z}
    }
}
impl ops::AddAssign<DbVector3> for DbVector3{
    fn add_assign(&mut self, rhs: DbVector3){
        let _ = self.x+rhs.x;
        let _ = self.y+rhs.y;
        let _ = self.z+rhs.z;
    }
}
impl ops::Add<f32> for DbVector3{
    type Output = Self;
    fn add(self, rhs: f32) -> Self::Output {
        Self::Output {x: self.x+rhs, y: self.y+rhs, z: self.z+rhs}
    }
}
impl ops::AddAssign<f32> for DbVector3{
    fn add_assign(&mut self, rhs: f32){
        let _ = self.x+rhs;
        let _ = self.y+rhs;
        let _ = self.z+rhs;
    }
}

impl ops::Sub<DbVector3> for DbVector3{
    type Output = Self;
    fn sub(self, rhs: DbVector3) -> Self::Output {
        Self::Output {x: self.x-rhs.x, y: self.y-rhs.y, z: self.z-rhs.z}
    }
}
impl ops::SubAssign<DbVector3> for DbVector3{
    fn sub_assign(&mut self, rhs: DbVector3){
        let _ = self.x-rhs.x;
        let _ = self.y-rhs.y;
        let _ = self.z-rhs.z;
    }
}
impl ops::Sub<f32> for DbVector3{
    type Output = Self;
    fn sub(self, rhs: f32) -> Self::Output {
        Self::Output {x: self.x-rhs, y: self.y-rhs, z: self.z-rhs}
    }
}
impl ops::SubAssign<f32> for DbVector3{
    fn sub_assign(&mut self, rhs: f32){
        let _ = self.x-rhs;
        let _ = self.y-rhs;
        let _ = self.z-rhs;
    }
}

impl ops::Mul<DbVector3> for DbVector3{
    type Output = Self;
    fn mul(self, rhs: DbVector3) -> Self::Output {
        Self::Output {x: self.x*rhs.x, y: self.y*rhs.y, z: self.z*rhs.z}
    }
}
impl ops::MulAssign<DbVector3> for DbVector3{
    fn mul_assign(&mut self, rhs: DbVector3){
        let _ = self.x*rhs.x;
        let _ = self.y*rhs.y;
        let _ = self.z*rhs.z;
    }
}
impl ops::Mul<f32> for DbVector3{
    type Output = Self;
    fn mul(self, rhs: f32) -> Self::Output {
        Self::Output {x: self.x*rhs, y: self.y*rhs, z: self.z*rhs}
    }
}
impl ops::Mul<DbQuaternion> for DbVector3{
    type Output = Self;
    fn mul(self, rhs: DbQuaternion) -> Self::Output {
        let num1: f64 = rhs.x as f64 * 2f64;
        let num2: f64 = rhs.y as f64 * 2f64;
        let num3: f64 = rhs.z as f64 * 2f64;
        let num4: f64 = rhs.x as f64 * num1;
        let num5: f64 = rhs.y as f64 * num2;
        let num6: f64 = rhs.z as f64 * num3;
        let num7: f64 = rhs.x as f64 * num2;
        let num8: f64 = rhs.x as f64 * num3;
        let num9: f64 = rhs.y as f64 * num3;
        let num10: f64 = rhs.w as f64 * num1;
        let num11: f64 = rhs.w as f64 * num2;
        let num12: f64 = rhs.w as f64 * num3;

        Self::Output {x: ((1f64 - (num5 + num6)) * self.x as f64 + (num7 - num12) * self.y as f64 + (num8 + num11) * self.z as f64) as f32,
                      y: ((num7 + num12) * self.x as f64 + (1f64 - (num4 + num6)) * self.y as f64 + (num9 - num10) * self.z as f64) as f32,
                      z: ((num8 - num11) * self.x as f64 + (num9 + num10) * self.y as f64 + (1f64 - (num4 + num5)) * self.z as f64) as f32}
    }
}
impl ops::MulAssign<f32> for DbVector3{
    fn mul_assign(&mut self, rhs: f32){
        let _ = self.x*rhs;
        let _ = self.y*rhs;
        let _ = self.z*rhs;
    }
}
impl ops::MulAssign<DbQuaternion> for DbVector3{
    fn mul_assign(&mut self, rhs: DbQuaternion) {
        let num1: f64 = rhs.x as f64 * 2f64;
        let num2: f64 = rhs.y as f64 * 2f64;
        let num3: f64 = rhs.z as f64 * 2f64;
        let num4: f64 = rhs.x as f64 * num1;
        let num5: f64 = rhs.y as f64 * num2;
        let num6: f64 = rhs.z as f64 * num3;
        let num7: f64 = rhs.x as f64 * num2;
        let num8: f64 = rhs.x as f64 * num3;
        let num9: f64 = rhs.y as f64 * num3;
        let num10: f64 = rhs.w as f64 * num1;
        let num11: f64 = rhs.w as f64 * num2;
        let num12: f64 = rhs.w as f64 * num3;
        let mut vec3: DbVector3 = DbVector3::new(0f32,0f32,0f32);
        vec3.x = ((1f64 - (num5 + num6)) * self.x as f64 + (num7 - num12) * self.y as f64 + (num8 + num11) * self.z as f64) as f32;
        vec3.y = ((num7 + num12) * self.x as f64 + (1f64 - (num4 + num6)) * self.y as f64 + (num9 - num10) * self.z as f64) as f32;
        vec3.z = ((num8 - num11) * self.x as f64 + (num9 + num10) * self.y as f64 + (1f64 - (num4 + num5)) * self.z as f64) as f32;
        self.x = vec3.x;
        self.y = vec3.y;
        self.z = vec3.z;
    }
}

impl ops::Div<DbVector3> for DbVector3{
    type Output = Self;
    fn div(self, rhs: DbVector3) -> Self::Output {
        Self::Output {x: self.x/rhs.x, y: self.y/rhs.y, z: self.z/rhs.z}
    }
}
impl ops::DivAssign<DbVector3> for DbVector3{
    fn div_assign(&mut self, rhs: DbVector3){
        let _ = self.x/rhs.x;
        let _ = self.y/rhs.y;
        let _ = self.z/rhs.z;
    }
}
impl ops::Div<f32> for DbVector3{
    type Output = Self;
    fn div(self, rhs: f32) -> Self::Output {
        Self::Output {x: self.x/rhs, y: self.y/rhs, z: self.z/rhs}
    }
}
impl ops::DivAssign<f32> for DbVector3{
    fn div_assign(&mut self, rhs: f32){
        let _ = self.x/rhs;
        let _ = self.y/rhs;
        let _ = self.z/rhs;
    }
}

impl ops::Rem<f32> for DbVector3{
    type Output = Self;
    fn rem(self, rhs: f32) -> Self::Output {
        DbVector3{x: self.x%rhs, y: self.y%rhs, z: self.z%rhs}
    }
}


