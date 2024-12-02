#pragma once

#include "include-muse\muse.h"

using namespace interaxon::bridge;

namespace MuseLibWrapper {
	
	class MuseListenerWindows : public interaxon::bridge::MuseListener
	{
	public:
		MuseListenerWindows() {}

		void muse_list_changed() override {

		}
	};
}

