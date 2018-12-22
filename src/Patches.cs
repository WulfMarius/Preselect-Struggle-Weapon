using Harmony;

namespace PreselectStruggleWeapon
{
    [HarmonyPatch(typeof(ItemDescriptionPage), "OnFavorite")]
    internal class ItemDescriptionPage_OnFavorite
    {
        public static bool Prefix(ItemDescriptionPage __instance)
        {
            var panelInventory = InterfaceManager.m_Panel_Inventory;
            var gearItem = (GearItem)AccessTools.Method(panelInventory.GetType(), "GetCurrentlySelectedGearItem").Invoke(panelInventory, null);

            if (!Implementation.IsStruggleWeapon(gearItem))
            {
                return true;
            }

            Implementation.TogglePreferredStruggleWeapon(gearItem);
            InterfaceManager.m_Panel_Inventory.RefreshTable();
            return false;
        }
    }

    [HarmonyPatch(typeof(ItemDescriptionPage), "UpdateFavoriteStatus")]
    internal class ItemDescriptionPage_UpdateFavoriteStatus
    {
        public static bool Prefix(ItemDescriptionPage __instance, GearItem gi)
        {
            var uiLocalize = __instance.m_FavoriteLabel.GetComponent<UILocalize>();

            if (Implementation.IsStruggleWeapon(gi))
            {
                bool isPreferredStruggleWeapon = Implementation.IsPreferredStruggleWeapon(gi);

                Utils.SetActive(__instance.m_FavoriteObject, true);
                Utils.SetActive(__instance.m_FavoriteCheckmark, isPreferredStruggleWeapon);
                __instance.m_FavoriteLabel.color = isPreferredStruggleWeapon ? __instance.m_FavoriteLabelColorChecked : __instance.m_FavoriteLabelColorUnchecked;
                __instance.m_FavoriteLabel.text = Localization.Get("GAMEPLAY_PreferredStruggleWeapon");

                if (uiLocalize)
                {
                    uiLocalize.enabled = false;
                }

                return false;
            }

            if (uiLocalize && !uiLocalize.enabled)
            {
                uiLocalize.enabled = true;
                __instance.m_FavoriteLabel.text = Localization.Get(uiLocalize.key);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "AddToExistingStackable", new System.Type[] { typeof(string), typeof(float), typeof(int), typeof(GearItem) })]
    internal class PlayerManager_AddToExistingStackable
    {
        public static void Postfix(GearItem __result, GearItem gearToAdd)
        {
            if (__result != null && Implementation.IsPreferredStruggleWeapon(gearToAdd))
            {
                Implementation.TogglePreferredStruggleWeapon(__result);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerStruggle), "WeaponPickerBegin")]
    internal class PlayerStruggle_WeaponPickerBegin
    {
        public static bool Prefix(PlayerStruggle __instance)
        {
            var preferredStruggleWeapon = Implementation.GetPreferredStruggleWeapon();
            if (preferredStruggleWeapon == null)
            {
                return true;
            }

            __instance.OnWeaponPicked(preferredStruggleWeapon);

            AccessTools.Method(__instance.GetType(), "PlayPickerExitAudio").Invoke(__instance, null);
            AccessTools.Method(__instance.GetType(), "DoChangeWeapon").Invoke(__instance, null);
            return false;
        }
    }

    [HarmonyPatch(typeof(SaveGameSystem), "RestoreGlobalData")]
    internal class SaveGameSystem_RestoreGlobalData
    {
        public static void Postfix(string name)
        {
            Implementation.LoadData(name);
        }
    }

    [HarmonyPatch(typeof(SaveGameSystem), "SaveGlobalData")]
    internal class SaveGameSystem_SaveGlobalData
    {
        public static void Postfix(SaveSlotType gameMode, string name)
        {
            Implementation.SaveData(gameMode, name);
        }
    }
}