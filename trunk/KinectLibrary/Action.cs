using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace KinectLibrary
{
    /// <summary>
    /// Similar to a Java swing Action this manages a button or verb
    /// </summary>
    public abstract class Action : INotifyPropertyChanged
    {
        private String m_Name;
        private bool m_Enabled;

        protected Action( )
        {
            Enabled = true;
        }

        protected Action(String name) : this()
        {
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected  virtual void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public virtual String Name
        {
            get { return m_Name; }
            set
            {
                if (value == m_Name)
                    return;
                m_Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public virtual bool Enabled
        {
            get { return m_Enabled; }
            set
            {
                if (value == m_Enabled)
                    return;
                m_Enabled = value;
                NotifyPropertyChanged("Enabled");
            }
        }

        public abstract void Fire();


    }
}
