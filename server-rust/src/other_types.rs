use spacetimedb::SpacetimeType;
use crate::db_quaternion::DbQuaternion;
use crate::db_vector2::DbVector2;
use crate::db_vector3::DbVector3;

#[derive(SpacetimeType, Clone, Copy, Debug)]
pub struct ControllerInput{
    pub position: DbVector3,
    pub rotation: DbQuaternion,
    pub joystick_position: DbVector2,
    pub is_ax_pressed: bool,
    pub was_ax_pressed_this_frame: bool,
    pub is_by_pressed: bool,
    pub was_by_pressed_this_frame: bool,
    pub index_trigger_value: f32,
    pub index_trigger_pressed_this_frame: bool,
    pub grip_trigger_value: f32,
    pub grip_trigger_pressed_this_frame: bool,
}

#[derive(SpacetimeType, Clone, Copy, Debug)]
pub enum TrackerType{
    Head,
    RightHand,
    LeftHand
}