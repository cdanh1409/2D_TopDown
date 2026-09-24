using UnityEngine;

namespace TopDownSurvival.Core
{
    public interface IInteractable
    {
        string GetInteractionPrompt();
        void Interact(GameObject actor);
    }
}
