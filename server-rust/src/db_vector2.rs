use std::ops;
use spacetimedb::SpacetimeType;

#[derive(SpacetimeType, Copy, Clone, Debug)]
pub struct DbVector2 {
    pub x: f32,
    pub y: f32,
}

impl DbVector2{
    pub fn new(x:f32, y:f32) -> DbVector2{
        DbVector2{x, y}
    }
    pub fn normalize(self) ->DbVector2{
        self/self.length()
    }
    pub fn length(self) -> f32{
        f32::sqrt(self.length_squared())
    }
    pub fn length_squared(self) -> f32{
        Self::dot(self, self)
    }
    pub fn dot(vector1: DbVector2, vector2: DbVector2) -> f32{
        (vector1.x * vector2.x) + (vector1.y * vector2.y)
    }
    pub fn distance(vector1: DbVector2, vector2: DbVector2) -> f32{
        f32::sqrt(Self::distance_squared(vector1, vector2))
    }
    pub fn distance_squared(vector1: DbVector2, vector2: DbVector2) -> f32{
        let diff = vector1 - vector2;
        DbVector2::dot(diff, diff)
    }
}

impl ops::Add<DbVector2> for DbVector2{
    type Output = Self;
    fn add(self, rhs: DbVector2) -> Self::Output {
        Self::Output {x: self.x+rhs.x, y: self.y+rhs.y}
    }
}
impl ops::AddAssign<DbVector2> for DbVector2{
    fn add_assign(&mut self, rhs: DbVector2){
        let _ = self.x+rhs.x;
        let _ = self.y+rhs.y;
    }
}
impl ops::Add<f32> for DbVector2{
    type Output = Self;
    fn add(self, rhs: f32) -> Self::Output {
        Self::Output {x: self.x+rhs, y: self.y+rhs}
    }
}
impl ops::AddAssign<f32> for DbVector2{
    fn add_assign(&mut self, rhs: f32){
        let _ = self.x+rhs;
        let _ = self.y+rhs;
    }
}

impl ops::Sub<DbVector2> for DbVector2{
    type Output = Self;
    fn sub(self, rhs: DbVector2) -> Self::Output {
        Self::Output {x: self.x-rhs.x, y: self.y-rhs.y}
    }
}
impl ops::SubAssign<DbVector2> for DbVector2{
    fn sub_assign(&mut self, rhs: DbVector2){
        let _ = self.x-rhs.x;
        let _ = self.y-rhs.y;
    }
}
impl ops::Sub<f32> for DbVector2{
    type Output = Self;
    fn sub(self, rhs: f32) -> Self::Output {
        Self::Output {x: self.x-rhs, y: self.y-rhs}
    }
}
impl ops::SubAssign<f32> for DbVector2{
    fn sub_assign(&mut self, rhs: f32){
        let _ = self.x-rhs;
        let _ = self.y-rhs;
    }
}

impl ops::Mul<DbVector2> for DbVector2{
    type Output = Self;
    fn mul(self, rhs: DbVector2) -> Self::Output {
        Self::Output {x: self.x*rhs.x, y: self.y*rhs.y}
    }
}
impl ops::MulAssign<DbVector2> for DbVector2{
    fn mul_assign(&mut self, rhs: DbVector2){
        let _ = self.x*rhs.x;
        let _ = self.y*rhs.y;
    }
}
impl ops::Mul<f32> for DbVector2{
    type Output = Self;
    fn mul(self, rhs: f32) -> Self::Output {
        Self::Output {x: self.x*rhs, y: self.y*rhs}
    }
}
impl ops::MulAssign<f32> for DbVector2{
    fn mul_assign(&mut self, rhs: f32){
        let _ = self.x*rhs;
        let _ = self.y*rhs;
    }
}

impl ops::Div<DbVector2> for DbVector2{
    type Output = Self;
    fn div(self, rhs: DbVector2) -> Self::Output {
        Self::Output {x: self.x/rhs.x, y: self.y/rhs.y}
    }
}
impl ops::DivAssign<DbVector2> for DbVector2{
    fn div_assign(&mut self, rhs: DbVector2){
        let _ = self.x/rhs.x;
        let _ = self.y/rhs.y;
    }
}
impl ops::Div<f32> for DbVector2{
    type Output = Self;
    fn div(self, rhs: f32) -> Self::Output {
        Self::Output {x: self.x/rhs, y: self.y/rhs}
    }
}
impl ops::DivAssign<f32> for DbVector2{
    fn div_assign(&mut self, rhs: f32){
        let _ = self.x/rhs;
        let _ = self.y/rhs;
    }
}


