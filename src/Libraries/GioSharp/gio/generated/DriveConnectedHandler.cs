// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;

	public delegate void DriveConnectedHandler(object o, DriveConnectedArgs args);

	public class DriveConnectedArgs : GLib.SignalArgs {
		public GLib.Drive Drive{
			get {
				return (GLib.Drive) Args[0];
			}
		}

	}
}
