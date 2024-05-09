using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoGame.Interactions
{
public interface IInteractable
{
        void ProceedInput();
        IEnumerator ReturnOutput();
}

}
