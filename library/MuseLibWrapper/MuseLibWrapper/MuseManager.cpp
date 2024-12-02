#pragma once

#include "MuseManager.h"
#include "Muse.h"
#include <iostream>

using namespace System;
using namespace System::Collections::Generic;
using namespace interaxon::bridge;

namespace MuseLibWrapper {

    // Exposing the native function via extern "C" for C# or other managed code to call
    extern "C" {
        MuseManagerWindows* Get_Muse_Manager() {
            return MuseManagerWindows::get_instance().get();
        }

        // Implementation of the native function to return a pointer to the Muse list
        std::vector<std::shared_ptr<Muse>>* Get_Muse_List() {
            auto museManager = MuseManagerWindows::get_instance(); // returns the MuseManager (singleton)
            if (museManager) {
                return new std::vector<std::shared_ptr<Muse>>(museManager->get_muses());
            }
            return nullptr;
        }
    }

    // Define the managed methods in the wrapper class
    System::IntPtr MuseManagerW::GetMuseManager() {
        return System::IntPtr(Get_Muse_Manager());
    }

    List<MuseW^>^ MuseManagerW::GetMuseList() {
        auto nativeList = Get_Muse_List();
        List<MuseW^>^ managedList = gcnew List<MuseW^>();

        if (nativeList) {
            for (auto& muse : *nativeList) {
                // Create a managed MuseW object from the native Muse pointer
                MuseW^ managedMuse = gcnew MuseW(muse.get());  // 'muse.get()' gives the native pointer
                managedList->Add(managedMuse);
            }
            delete nativeList; // Clean up the native memory
        }

        return managedList;
    }

}