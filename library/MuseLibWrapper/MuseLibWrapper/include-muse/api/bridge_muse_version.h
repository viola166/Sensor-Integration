// AUTOGENERATED FILE - DO NOT MODIFY!
// This file generated by Djinni from museinfo.djinni

#pragma once

#include <cstdint>
#include <memory>
#include <string>

namespace interaxon { namespace bridge {

/**
 * Provides access to Muse firmware and hardware versions.<br>
 *
 * You must connect to the headband at least once to before this information
 * is available.  Once you have connected once the information will remain
 * available, even after you disconnect.
 */
class MuseVersion {
public:
    virtual ~MuseVersion() {}

    /** Internal use only.  Create a default version. */
    static std::shared_ptr<MuseVersion> make_default_version();

    /** Internal use only.  Create version from JSON string on \muse2016 or later. */
    static std::shared_ptr<MuseVersion> make_version(const std::string & json);

    /**
     * Provides access to the running state.
     * \li For \muse2014 this is one of \c app, \c bootloader or \c test.
     * \li For \muse2016 or later this is one of \c headband or \c bootloader.
     * \return The current running state of the headband.
     */
    virtual std::string get_running_state() const = 0;

    /**
     * Provides access to hardware version.
     * \return The hardware version.
     */
    virtual std::string get_hardware_version() const = 0;

    /**
     * BSP (board support package) version.
     * This is only available on \muse2016 or later.
     * \return The BSP version.
     */
    virtual std::string get_bsp_version() const = 0;

    /**
     * Provides access to the firmware version.
     * \return The firmware version.
     */
    virtual std::string get_firmware_version() const = 0;

    /**
     * Provides access to Muse bootloader version.
     * \return The bootloader version.
     */
    virtual std::string get_bootloader_version() const = 0;

    /**
     * Provides access to Muse firmware build number.
     * This is only available on \muse2014.
     * \return The firmware build number.
     */
    virtual std::string get_firmware_build_number() const = 0;

    /**
     * Type of firmware.  One of \c consumer, \c research or \c test.
     * \return The type of firmware.
     */
    virtual std::string get_firmware_type() const = 0;

    /**
     * Provides access to %Muse communication protocol version.
     * \return The communication protocol version.
     */
    virtual int32_t get_protocol_version() const = 0;
};

} }  // namespace interaxon::bridge