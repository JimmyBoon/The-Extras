using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extras.Objects;
using Extras.Cinematics;
using Extras.UI;


namespace Extras.Character
{
    public class CharacterTalking : MonoBehaviour
    {
        [SerializeField] float wantsToTalkTriggerValue = 60f;
        [SerializeField] float conversationLength = 8f;
        [SerializeField] bool social = false;
        [SerializeField] bool inConversation = false;
        [SerializeField] bool conversationInitiator = false;
        [SerializeField] bool followed = false;

        CharacterTalking otherCharacter = null;
        
        CharacterNeeds needs;
        CharacterMotion motion;
        CharacterEyeSight eyeSight;
        Animator animator;
        FollowChanger followChanger;
        TalkingUI talkingUI;

        public event Action<ActionType> TalkingStatusUpdated;

        private void Awake()
        {
            needs = GetComponent<CharacterNeeds>();
            motion = GetComponent<CharacterMotion>();
            eyeSight = GetComponent<CharacterEyeSight>();
            animator = GetComponent<Animator>(); 
            followChanger = FindObjectOfType<FollowChanger>();  
            talkingUI = GetComponentInChildren<TalkingUI>();         
        }

        public float GetConversationLength()
        {
            return conversationLength;
        }

        public CharacterTalking GetOtherCharacter()
        {
            return otherCharacter;
        }

        public bool EvaluateTalking()
        {
            ObjectOfInterest possibleCharacter = eyeSight.Look();

            if (possibleCharacter != null && possibleCharacter.GetObjectType() == ObjectType.Character 
            && possibleCharacter.gameObject != this.gameObject 
            && needs.GetSocialPriority() > wantsToTalkTriggerValue
            && possibleCharacter.GetComponent<CharacterTalking>().WantsToTalk())
            {
                otherCharacter = possibleCharacter.GetComponent<CharacterTalking>();
                return true;
            }
            else
            {
                otherCharacter = null;
                return false;
            }
            
        }

        public void InitiateConversation()
        {
            if(otherCharacter == null) { return; }

            social = true;
            conversationInitiator = true;

            TalkingStatusUpdated?.Invoke(ActionType.Social);

            motion.CancelDestination();

            otherCharacter.motion.CancelDestination();
            otherCharacter.SetOtherCharacter(this.gameObject.GetComponent<CharacterTalking>());
            otherCharacter.SetTalking(ActionType.Social);

            otherCharacter.SetTalkingDuration(this.conversationLength);

            if(followChanger.GetCharacterBeginFollowed() == gameObject || followChanger.GetCharacterBeginFollowed() == otherCharacter.gameObject)
            {
                followed = true;
                otherCharacter.SetOtherCharacterFollowed(true);
            }

            talkingUI.LaunchText("Hello There");

        }

        public void BeginConversation()
        {
            needs.SetNeed(ObjectType.Character, 100f);
            motion.CancelDestination();
            transform.LookAt(otherCharacter.transform.position);

            TalkingStatusUpdated?.Invoke(ActionType.Conversation);
            inConversation = true;
            social = false;

            animator.SetBool("talking", true);
            animator.SetFloat("talkValue", UnityEngine.Random.Range(0.0f, 1f));

        }

        public IEnumerator InConversation()
        {
            yield return new WaitForSeconds(1f);

            if(followed)
            {
                SetupUI();
            }
            yield return new WaitForSeconds(conversationLength);
            animator.SetBool("talking", false);
            inConversation = false;
            followed = false;
            otherCharacter = null;

        }

        public bool WantsToTalk()
        {
            return needs.GetSocialPriority() > wantsToTalkTriggerValue && social == false && inConversation == false;
        }

        public void SetTalking(ActionType actionType)
        {
            social = true;
            this.TalkingStatusUpdated?.Invoke(actionType);
        }

        public void SetOtherCharacter(CharacterTalking character)
        {
            otherCharacter = character;
        }

        public void SetOtherCharacterFollowed(bool beginFollowed)
        {
            followed = beginFollowed;
        }

        public void SetTalkingDuration(float duration)
        {
            conversationLength = duration;
        }

        private void SetupUI()
        {
            Debug.Log("SetupUI triggered");
            Vector3 myScreenPos = Camera.main.WorldToScreenPoint(transform.position);
            //Vector3 otherCharacterScreenPos = Camera.main.WorldToScreenPoint(otherCharacter.transform.position);
            talkingUI.LaunchText($"My Name is {gameObject.name}");
        }
    }
}