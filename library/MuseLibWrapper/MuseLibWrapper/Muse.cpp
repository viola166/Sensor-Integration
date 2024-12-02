#pragma once

#include "Muse.h"
#include <iostream>

using namespace System;
using namespace System::Collections::Generic;
using namespace interaxon::bridge;

namespace MuseLibWrapper {
	
	void MuseW::ConnectToMuse() {
		Muse* muse = (Muse*)my_muse.ToPointer();
		if (muse) {
			muse->connect();
		}
	}
}
