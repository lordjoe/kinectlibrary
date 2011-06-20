using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectLibrary
{

    // Summary:
    //     Provides data for the System.ComponentModel.INotifyPropertyChanged.PropertyChanged
    //     event.
    public class ActionFiredEventArgs : EventArgs
    {
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.PropertyChangedEventArgs
        //     class.
        //
        // Parameters:
        //   propertyName:
        //     The name of the property that changed.
        public ActionFiredEventArgs(Action wasFired)
        {
            WasFired = wasFired;
        }

        // Summary:
        //     Gets the name of the property that changed.
        //
        // Returns:
        //     The name of the property that changed.
          public virtual Action WasFired { get; private set; }
    }

    // Summary:
    //     Represents the method that will handle the System.ComponentModel.INotifyPropertyChanged.PropertyChanged
    //     event raised when a property is changed on a component.
    //
    // Parameters:
    //   sender:
    //     The source of the event.
    //
    //   e:
    //     A System.ComponentModel.PropertyChangedEventArgs that contains the event
    //     data.
    public delegate void ActionFiredEventEventHandler(object sender, ActionFiredEventArgs e);

    /// <summary>
    /// Implemented by a class that fires actions
    /// </summary>
    public interface IActionFired
    {
        // Summary:
        //     Occurs when a property value changes.
        event ActionFiredEventEventHandler ActionFired;

    }
}
