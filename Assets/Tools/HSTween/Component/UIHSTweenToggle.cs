using UnityEngine;
using UnityEngine.EventSystems;

namespace HSTween
{
    public class UIHSTweenToggle : MonoBehaviour, IPointerClickHandler
    {
        public GameObject AnimTarget;
        public string AnimKey;

        [Tooltip("컴포넌트 활성화시 오토 플레이 여부")]
        public bool AutoPlay = true;//Enable 실행시킬지 여부.
        [Tooltip("컴포넌트 활성화시 false : LastFrame, true : Forwards 실행시킬지 여부")]//'컴포넌트 활성화시 오토 플레이 여부'라고 적혀있지만 실제로는 다르게 동작중.
        public bool EnablePlay = false;
        [Tooltip("컴포넌트 활성화시 상태 여부")]
        public bool EnableBaseState = true;

        public UnityEngine.UI.Button.ButtonClickedEvent onToggle;

        public bool IsOn
        {
            get
            {
                return HSTweenUtility.IsForward(AnimTarget, AnimKey);
            }
        }

        private void OnEnable()
        {
            if(AutoPlay)
                PlayState(AnimKey, EnablePlay, EnableBaseState, true);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            OnToggle();
        }

        private void PlayState(string animKey, bool playAnim, bool force = false)
        {
            bool isState = HSTweenUtility.IsForward(AnimTarget, animKey);
            PlayState(animKey, playAnim, isState, force);
        }

        private void PlayState(string animKey, bool playAnim, bool state, bool force)
        {
            if (playAnim)
            {
                if (force)
                {
                    HSTweenUtility.Play(AnimTarget, AnimKey, state ? ePlayMode.Forwards : ePlayMode.Backwards, EndToggle);
                }
                else
                {
                    HSTweenUtility.Play(AnimTarget, AnimKey, state ? ePlayMode.Forwards_At_Current : ePlayMode.Backwards_At_Current, EndToggle);
                }
            }
            else
            {
                HSTweenUtility.Play(AnimTarget, AnimKey, state ? ePlayMode.LastFrame : ePlayMode.FirstFrame, EndToggle);
            }
        }

        public void OnToggle()
        {
            OnToggle(AnimKey);
        }

        public void OnToggle(string animKey)
        {
            PlayState(animKey, true);
        }

        private void EndToggle()
        {
            if (onToggle != null)
                onToggle.Invoke();
        }

        public void SetOnState()
        {
            HSTweenUtility.Play(AnimTarget, AnimKey, ePlayMode.Forwards, EndToggle);
        }
    }
}