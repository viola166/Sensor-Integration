#pragma once
#pragma managed

#include "include-muse\muse.h"
#include <memory>
#include <vector>

using namespace System;
using namespace System::Collections::Generic;
using namespace interaxon::bridge; 

namespace MuseLibWrapper {

    public ref class MuseW {

    private:
        System::IntPtr my_muse;

    public:
        // Managed method to return a pointer to the Muse instance
        MuseW(Muse* muse) {
            my_muse = IntPtr(muse);
        }

        Muse* GetNativeMuse() {
            return (Muse*)my_muse.ToPointer();
        }

        void ConnectToMuse();
    };

}