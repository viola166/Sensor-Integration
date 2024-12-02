// AUTOGENERATED FILE - DO NOT MODIFY!
// This file generated by Djinni from packets.djinni

#pragma once

#include <functional>

namespace interaxon { namespace bridge {

/**
 * \if IOS_ONLY
 * \file
 * \endif
 * Represents the data mapping in an Accelerometer data packet.<br>
 *
 * The accelerometer data is measured on 3 axes as shown in the picture below:
 * \image html MuseAxesAcc.png
 *
 * The axes are oriented to a Right Hand Coordinate System along the headband
 * axes. Values are given in g (9.81 m/s^2) and are negated to align
 * with the orientation of the headband in Earth's gravitational field.
 * This convention is described in more detail in the following application note:
 * https://www.nxp.com/files/sensors/doc/app_note/AN3461.pdf
 * <br>
 * Values along the X axis increase as the head tilts down aligning the X axis with
 * the downward force of gravity.  Negative values indicate a tilt upwards.<br>
 * Values along the Y axis increase as the head tilts to the right.
 * Negative values indicate a tilt to the left.<br>
 * When worn on a level head, or held in the level position shown in the figure above,
 * the net acceleration of the device will only be caused from gravity. It will be in the
 * direction of the ground aligned with the Z axis. This will give a_x =0, a_y = 0 and a_z = 1 g.
 * As the headband tilts out of this orientation, the value of Z will decrease.  -1 in Z represents
 * a headband that is upside down.<br>
 * \sa \enumlink{MuseDataPacketType,ACCELEROMETER,IXNMuseDataPacketTypeAccelerometer}
 * \sa \functionlink{MuseDataPacket,getAccelerometerValue,get_accelerometer_value}
 */
enum class Accelerometer : int {
    /**
     * Orientation of the X axis relative to gravity in g.
     * Values along the X axis increase as the head tilts down.
     * Negative values indicate a tilt up.<br>
     */
    X,
    /**
     * Orientation of the Y axis relative to gravity in g.
     * Values along the Y axis increase as the head tilts to the right.
     * Negative values indicate a tilt to the left.<br>
     */
    Y,
    /** Orientation of the Z axis relative to gravity in g.<br> */
    Z,
};

} }  // namespace interaxon::bridge

namespace std {

template <>
struct hash<::interaxon::bridge::Accelerometer> {
    size_t operator()(::interaxon::bridge::Accelerometer type) const {
        return std::hash<int>()(static_cast<int>(type));
    }
};

}  // namespace std
