using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectSystem
{
    public interface IObject
    {
        public void Initialized();
        public void Draw();
        public ObjectState Evaluator();
    }
}
