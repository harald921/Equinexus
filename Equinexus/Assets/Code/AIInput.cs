using UnityEngine;
using System.Collections;

public class AIInput : MonoBehaviour 
{
    [System.Serializable]
    public enum Type
    {
        Rusher
    }

    [SerializeField] Type _type;

    Character _target = null;

    void Update()
    {
        switch (_type)
        {
            case Type.Rusher:
                HandleRusherAI();
                break;

            default:
                break;
        }
    }

    void HandleRusherAI()
    {
        Character.Input input = new Character.Input(Quaternion.identity, Vector3.zero, false, false, false, false, false);

        if (!_target)
        {
            Character[] charactersInScene = FindObjectsOfType<Character>();

            foreach (Character otherCharacter in charactersInScene)
                if (otherCharacter != GetComponent<Character>())
                    if (otherCharacter.Team != GetComponent<Character>().Team)
                        _target = otherCharacter;
        }

        else
        {
            input.targetRotation = Quaternion.LookRotation(_target.transform.position - transform.position);
            input.moveDirection = transform.forward;
        }

        GetComponent<Character>().input = input;
    }
}
