using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts
{
    public class SoundManager : SerializedMonoBehaviour
    {
        public enum Music
        {
            Theme1,    
            Theme2,    
            Pause
        }
    
        public enum Sfx
        {
            ButtonClick,
            PopupDismiss,
            MenuSwap,
        
            ItemSold,

            CombatStart,
            CombatEnd,
        
            Recruit1,
            Recruit2,
            Recruit3,
            Recruit4,
            Recruit5,
            Recruit6,
            Recruit7,
            Recruit8,
            Recruit9,
            Recruit10,
            Recruit11,
            Recruit12,
            Recruit13,
            Recruit14,
            Recruit15,
            Recruit16,
            
            ItemEquipped,
            ItemUnEquipped,
        }

        [Serializable]
        public struct MusicEntry
        {
            public Music Type;
            public AudioClip Clip;
        }
    
        [Serializable]
        public struct SFXEntry
        {
            public Sfx Type;
            public AudioClip Clip;
        }

        public float MusicVolume { get; set; } = 0.05f;
        public float SfxVolume { get; set; } = 1.0f;

        [SerializeField] private AudioSource m_musicAudioSource;
        [SerializeField] private AudioSource m_sfxAudioSource;

        [SerializeField] private Dictionary<Music, AudioClip> m_musicMapping = new Dictionary<Music, AudioClip>();
        [SerializeField] private Dictionary<Sfx, AudioClip> m_sfxMapping = new Dictionary<Sfx, AudioClip>();

        private void Awake()
        {
            ApplySettings();
            
            RequestMusic(Music.Theme1);
        }

        public void ApplySettings()
        {
            m_musicAudioSource.volume = MusicVolume;
            m_sfxAudioSource.volume = SfxVolume;
        }

        public void RequestMusic(Music type)
        {
            m_musicAudioSource.clip = m_musicMapping[type];
            m_musicAudioSource.loop = true;
            m_musicAudioSource.Play();
        }
    
        public void RequestSfx(Sfx type)
        {
            if (!m_sfxMapping.ContainsKey(type))
            {
                return;
            }
        
            m_sfxAudioSource.PlayOneShot(m_sfxMapping[type]);
        }
    }
}
