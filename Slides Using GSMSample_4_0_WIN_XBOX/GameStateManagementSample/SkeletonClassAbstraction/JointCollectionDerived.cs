using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace GameStateManagement
{
    public class JointCollectionDerived //: JointCollection
    {
        public JointCollectionDerived() //: base()
        {
            
        }
        public Joint this[JointType i]
        {
            get
            {
                return this[ i];
            }
            set
            {
                this[ i] = value;
            }
        }
    }
}
