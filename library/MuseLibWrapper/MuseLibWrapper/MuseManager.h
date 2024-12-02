#pragma once
#pragma managed

#include "include-muse\muse.h"
#include "Muse.h"
#include <memory>
#include <vector>

using namespace System;
using namespace System::Collections::Generic;

namespace MuseLibWrapper {

    public ref class MuseManagerW {

    public:
        // Managed method to return a pointer to the MuseManager instance
        System::IntPtr GetMuseManager();

        List<MuseW^>^ GetMuseList();
    };


    // Declaration of the native function, exported from the C++ code
    extern "C" {
        __declspec(dllexport) interaxon::bridge::MuseManagerWindows* Get_Muse_Manager();

        __declspec(dllexport) std::vector<std::shared_ptr<interaxon::bridge::Muse>>* Get_Muse_List();

    }
}