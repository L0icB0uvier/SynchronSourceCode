/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TextWriter : MonoBehaviour {

        private static TextWriter instance;

        private List<TextWriterSingle> textWriterSingleList;

        private void Awake() {
            instance = this;
            textWriterSingleList = new List<TextWriterSingle>();
        }

        public static TextWriterSingle AddWriter_Static(TMP_Text uiText, string textToWrite, float timePerCharacter, bool
            invisibleCharacters, bool writeReverse, bool removeWriterBeforeAdd, Action onComplete) {
            if (removeWriterBeforeAdd) {
                instance.RemoveWriter(uiText);
            }
            return instance.AddWriter(uiText, textToWrite, timePerCharacter, invisibleCharacters, writeReverse, onComplete);
        }
        public static TextWriterSingle AddWriter_Static(TMP_Text uiText, string textToWrite, float timePerCharacter, bool
            invisibleCharacters, bool writeReverse, bool removeWriterBeforeAdd) {
            if (removeWriterBeforeAdd) {
                instance.RemoveWriter(uiText);
            }
            return instance.AddWriter(uiText, textToWrite, timePerCharacter, invisibleCharacters, writeReverse);
        }

        private TextWriterSingle AddWriter(TMP_Text uiText, string textToWrite, float timePerCharacter, bool 
            invisibleCharacters, bool writeReverse, Action onComplete) {
            TextWriterSingle textWriterSingle = new TextWriterSingle(uiText, textToWrite, timePerCharacter, 
                invisibleCharacters, writeReverse, onComplete);
            textWriterSingleList.Add(textWriterSingle);
            return textWriterSingle;
        } 
        private TextWriterSingle AddWriter(TMP_Text uiText, string textToWrite, float timePerCharacter, bool 
            invisibleCharacters, bool writeReverse) {
            TextWriterSingle textWriterSingle = new TextWriterSingle(uiText, textToWrite, timePerCharacter, 
                invisibleCharacters, writeReverse);
            textWriterSingleList.Add(textWriterSingle);
            return textWriterSingle;
        }

        public static void RemoveWriter_Static(TMP_Text uiText) {
            instance.RemoveWriter(uiText);
        }

        private void RemoveWriter(TMP_Text uiText) {
            for (int i = 0; i < textWriterSingleList.Count; i++) {
                if (textWriterSingleList[i].GetUIText() == uiText) {
                    textWriterSingleList.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Update() {
            for (int i = 0; i < textWriterSingleList.Count; i++) {
                bool destroyInstance = textWriterSingleList[i].Update();
                if (destroyInstance) {
                    textWriterSingleList.RemoveAt(i);
                    i--;
                }
            }
        }

        /*
     * Represents a single TextWriter instance
     * */
        public class TextWriterSingle {

            private TMP_Text uiText;
            private string textToWrite;
            private int characterIndex;
            private float timePerCharacter;
            private float timer;
            private bool invisibleCharacters;
            private bool writeReverse;
            private Action onComplete;

            public TextWriterSingle(TMP_Text uiText, string textToWrite, float timePerCharacter, bool 
                invisibleCharacters, bool writeReverse, Action onComplete) {
                this.uiText = uiText;
                this.textToWrite = textToWrite;
                this.timePerCharacter = timePerCharacter;
                this.invisibleCharacters = invisibleCharacters;
                this.writeReverse = writeReverse;
                this.onComplete = onComplete;
                characterIndex = 0;
            }
        
            public TextWriterSingle(TMP_Text uiText, string textToWrite, float timePerCharacter, bool 
                invisibleCharacters, bool writeReverse) {
                this.uiText = uiText;
                this.textToWrite = textToWrite;
                this.timePerCharacter = timePerCharacter;
                this.invisibleCharacters = invisibleCharacters;
                this.writeReverse = writeReverse;
                characterIndex = 0;
            }

            // Returns true on complete
            public bool Update() {
                timer -= Time.deltaTime;
                while (timer <= 0f) {
                    // Display next character
                    timer += timePerCharacter;
                    characterIndex++;

                    string text;

                    if (writeReverse)
                    {
                        text = textToWrite.Substring(textToWrite.Length - characterIndex, characterIndex);
                        if (invisibleCharacters) {
                            text += "<color=#00000000>" + textToWrite.Substring(characterIndex) + "</color>";
                        }
                    }

                    else
                    {
                        text = textToWrite.Substring(0, characterIndex);
                        if (invisibleCharacters) {
                            text += "<color=#00000000>" + textToWrite.Substring(characterIndex) + "</color>";
                        }
                    }
                 
                    uiText.text = text;

                    if (characterIndex >= textToWrite.Length) {
                        // Entire string displayed
                        if (onComplete != null) onComplete();
                        return true;
                    }
                }

                return false;
            }

            public TMP_Text GetUIText() {
                return uiText;
            }

            public bool IsActive() {
                return characterIndex < textToWrite.Length;
            }

            public void WriteAllAndDestroy() {
                uiText.text = textToWrite;
                characterIndex = textToWrite.Length;
                if (onComplete != null) onComplete();
                TextWriter.RemoveWriter_Static(uiText);
            }


        }


    }
}
