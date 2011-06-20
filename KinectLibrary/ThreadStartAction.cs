using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KinectLibrary
{
    public class ThreadStartAction : Action
    {
        private ThreadStart m_ToDo;
        public ThreadStartAction(String name, ThreadStart action)
            : base(name)
        {
            m_ToDo = action;
            
        }

        override
        public void Fire()
        {
            m_ToDo.Invoke();
        }
    }
}
