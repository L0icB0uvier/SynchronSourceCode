using Audio.SFXPlayers.EnvironmentalSFXPlayers.GameplayElementsSFXPlayers;
using UnityEngine;

namespace Audio
{
    public class DoorAudio : GameplayElementAudio
    {
        [Header("Audios")]
        [SerializeField] private AudioCueSO _openDoor;
        [SerializeField] private AudioCueSO _closeDoor;

        public void PlayOpenDoorSound() => PlayAudio(_openDoor);
        public void PlayCloseDoorSound() => PlayAudio(_closeDoor);
    }
}