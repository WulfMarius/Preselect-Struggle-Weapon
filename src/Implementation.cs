using UnityEngine;
using System.Reflection;

namespace PreselectStruggleWeapon
{
    public class Implementation
    {
        public const string SAVE_SLOT_NAME = "PreselectStruggleWeapon";

        private const string NAME = "Preselect-Struggle-Weapon";

        public static int PreferredStruggleWeaponId
        {
            get;
            private set;
        }

        public static GearItem GetPreferredStruggleWeapon()
        {
            return GameManager.GetInventoryComponent().FindByInstanceID(PreferredStruggleWeaponId);
        }

        public static bool IsPreferredStruggleWeapon(GearItem gearItem)
        {
            if (gearItem == null)
            {
                return false;
            }

            return PreferredStruggleWeaponId == gearItem.m_InstanceID;
        }

        public static bool IsStruggleWeapon(GearItem gearItem)
        {
            return gearItem != null && gearItem.m_StruggleBonus != null && gearItem.m_StruggleBonus.m_StruggleWeaponType != StruggleBonus.StruggleWeaponType.BareHands;
        }

        public static void OnLoad()
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            Log("Version " + assemblyName.Version);

            AddTranslations();
        }

        public static void TogglePreferredStruggleWeapon(GearItem gearItem)
        {
            if (IsPreferredStruggleWeapon(gearItem))
            {
                PreferredStruggleWeaponId = 0;
            }
            else
            {
                PreferredStruggleWeaponId = gearItem.m_InstanceID;
            }
        }

        internal static void LoadData(string saveSlotName)
        {
            string json = SaveGameSlots.LoadDataFromSlot(saveSlotName, SAVE_SLOT_NAME);
            if (string.IsNullOrEmpty(json))
            {
                PreferredStruggleWeaponId = 0;
                return;
            }

            SaveData saveData = Utils.DeserializeObject<SaveData>(json);
            PreferredStruggleWeaponId = saveData.PreferredStruggleWeapon;
        }

        internal static void Log(string message)
        {
            Debug.LogFormat("[" + NAME + "] {0}", message);
        }

        internal static void Log(string message, params object[] parameters)
        {
            string preformattedMessage = string.Format("[" + NAME + "] {0}", message);
            Debug.LogFormat(preformattedMessage, parameters);
        }

        internal static void SaveData(SaveSlotType gameMode, string saveSlotName)
        {
            SaveData saveData = new SaveData();
            saveData.PreferredStruggleWeapon = PreferredStruggleWeaponId;

            string json = Utils.SerializeObject(saveData);
            SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, saveSlotName, SAVE_SLOT_NAME, json);
        }

        private static void AddTranslations()
        {
            string[] knownLanguages = Localization.knownLanguages;
            string[] translations = new string[knownLanguages.Length];
            for (int i = 0; i < knownLanguages.Length; i++)
            {
                switch (knownLanguages[i])
                {
                    case "English":
                        translations[i] = "Struggle Weapon";
                        break;

                    case "German":
                        translations[i] = "Nahkampfwaffe";
                        break;

                    case "Russian":
                        translations[i] = "Основное оружие";
                        break;

                    default:
                        translations[i] = "Struggle Weapon\nHelp me translate this!\nVisit https://github.com/WulfMarius/Preselect-Struggle-Weapon";
                        break;
                }
            }

            Localization.dictionary.Add("GAMEPLAY_PreferredStruggleWeapon", translations);
        }
    }

    internal class SaveData
    {
        public int PreferredStruggleWeapon;
    }
}