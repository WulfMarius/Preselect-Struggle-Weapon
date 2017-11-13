using Newtonsoft.Json;

namespace PreselectStruggleWeapon
{
    internal class Implementation
    {
        public const string SAVE_SLOT_NAME = "PreselectStruggleWeapon";

        public static int PreferredStruggleWeaponId;

        public static void OnLoad()
        {
            UnityEngine.Debug.Log("[Preselect-Struggle-Weapon]: Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

            AddTranslations();
        }

        internal static bool IsPreferredStruggleWeapon(GearItem gearItem)
        {
            if (gearItem == null)
            {
                return false;
            }

            return PreferredStruggleWeaponId == gearItem.m_InstanceID;
        }

        internal static bool IsStruggleWeapon(GearItem gearItem)
        {
            return gearItem != null && gearItem.m_StruggleBonus != null && gearItem.m_StruggleBonus.m_StruggleWeaponType != StruggleBonus.StruggleWeaponType.BareHands;
        }

        internal static void LoadData(string saveSlotName)
        {
            string value = SaveGameSlots.LoadDataFromSlot(saveSlotName, SAVE_SLOT_NAME);
            if (string.IsNullOrEmpty(value))
            {
                PreferredStruggleWeaponId = 0;
                return;
            }

            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(value);
            PreferredStruggleWeaponId = saveData.PreferredStruggleWeapon;
        }

        internal static void SaveData(string saveSlotName)
        {
            SaveData saveData = new SaveData();
            saveData.PreferredStruggleWeapon = PreferredStruggleWeaponId;

            SaveGameSlots.SaveDataToSlot(saveSlotName, SAVE_SLOT_NAME, JsonConvert.SerializeObject(saveData));
        }

        internal static void TogglePreferredStruggleWeapon(GearItem gearItem)
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