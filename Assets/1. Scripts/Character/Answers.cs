using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extras.Character
{
    [CreateAssetMenu(fileName = "Answers", menuName = "Conversation/Create Answers", order = 0)]
    public class Answers : ScriptableObject
    {
        [SerializeField] List<string> answers = new List<string>();

        public List<string> GetAnswers()
        {
            return answers;
        }
    }
}