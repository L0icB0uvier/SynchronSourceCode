using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CallToActionUI
{
    public class FixedActionUI : ActionUI
    {
        [SerializeField]
        private Image bar;
    
        public TMP_Text text1;

        public TMP_Text text2;

        public float timePerCharacter = .1f;
        public bool writeReverse;

        [SerializeField]
        private GameObject textGroup;

        [SerializeField]
        private AudioSource audioSource;

        private float m_currentFadeDuration;

        [SerializeField]
        private float fadeDuration = .5f;

        [SerializeField]
        private float fadedOutAlpha;
    
        [SerializeField]
        private float fadedInAlpha;

        private string m_currentText;
        public string CurrentText => m_currentText;

        private void Awake()
        {
            HideUI();
        }

        public override void ChangeText(string text, ActionUISettings actionUISettings)
        {
            if (!textGroup.activeInHierarchy) ShowUI();

            if (m_currentText == text) return;
        
            m_currentText = text;
            text1.text = "";
            text2.text = "";
            string[] textSplitted = text.Split(char.Parse(" "));
            if (textSplitted[0].Length >= textSplitted[1].Length)
            {
                TextWriter.AddWriter_Static(text1, textSplitted[0], timePerCharacter, false, writeReverse, true, OnComplete);
                TextWriter.AddWriter_Static(text2, textSplitted[1], timePerCharacter, false, writeReverse, true);
            }

            else
            {
                TextWriter.AddWriter_Static(text1, textSplitted[0], timePerCharacter, false, writeReverse, true);
                TextWriter.AddWriter_Static(text2, textSplitted[1], timePerCharacter, false, writeReverse, true, OnComplete);
            }
        
            audioSource.Play();
        }
        
        /*public override void ChangeText(string text)
        {
            if (!textGroup.activeInHierarchy) ShowUI();

            if (m_currentText == text) return;
        
            m_currentText = text;
            text1.text = "";
            text2.text = "";
            string[] textSplitted = text.Split(char.Parse(" "));
            if (textSplitted[0].Length >= textSplitted[1].Length)
            {
                TextWriter.AddWriter_Static(text1, textSplitted[0], timePerCharacter, false, writeReverse, true, OnComplete);
                TextWriter.AddWriter_Static(text2, textSplitted[1], timePerCharacter, false, writeReverse, true);
            }

            else
            {
                TextWriter.AddWriter_Static(text1, textSplitted[0], timePerCharacter, false, writeReverse, true);
                TextWriter.AddWriter_Static(text2, textSplitted[1], timePerCharacter, false, writeReverse, true, OnComplete);
            }
        
            audioSource.Play();
        }*/

        IEnumerator FadeBar(float from, float to)
        {
            m_currentFadeDuration = 0;
            float t = 0;
            var color = bar.color;
            while (m_currentFadeDuration < fadeDuration)
            {
                m_currentFadeDuration += Time.deltaTime;
                t = Mathf.Clamp01(m_currentFadeDuration / fadeDuration);
                color.a = Mathf.Lerp(from, to, t);
                bar.color = color;
                yield return null;
            }
        }

        private void OnComplete()
        {
            audioSource.Stop();
        }
    
        public void ShowUI()
        {
            StopAllCoroutines();
            textGroup.SetActive(true);
            StartCoroutine(FadeBar(fadedOutAlpha, fadedInAlpha));
        }
    
        public override void HideUI()
        {
            StopAllCoroutines();
            textGroup.SetActive(false);
            m_currentText = null;
            StartCoroutine(FadeBar(fadedInAlpha, fadedOutAlpha));
        }
    }
}
