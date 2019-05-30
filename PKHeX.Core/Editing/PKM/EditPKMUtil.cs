﻿using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    public static class EditPKMUtil
    {
        public static List<string> GetSuggestionMessage(PKM pkm, int level, int location, int minlvl)
        {
            var suggestion = new List<string> { MsgPKMSuggestionStart };
            if (pkm.Format >= 3)
            {
                var met_list = GameInfo.GetLocationList((GameVersion)pkm.Version, pkm.Format, egg: false);
                var locstr = met_list.First(loc => loc.Value == location).Text;
                suggestion.Add($"{MsgPKMSuggestionMetLocation} {locstr}");
                suggestion.Add($"{MsgPKMSuggestionMetLevel} {level}");
            }
            if (pkm.CurrentLevel < minlvl)
                suggestion.Add($"{MsgPKMSuggestionLevel} {minlvl}");
            return suggestion;
        }

        public static IReadOnlyList<ComboItem> GetAbilityList(PKM pkm)
        {
            var abils = pkm.PersonalInfo.Abilities;
            return GameInfo.GetAbilityList(abils, pkm.Format);
        }

        /// <summary>
        /// Applies junk data to a <see cref="SaveFile.BlankPKM"/>, which is preferable to a completely empty entity.
        /// </summary>
        /// <param name="pk">Blank data</param>
        /// <param name="tr">Trainer info to apply</param>
        public static void TemplateFields(PKM pk, ITrainerInfo tr)
        {
            pk.Move1 = 1;
            pk.Move1_PP = 40;
            pk.Ball = 4;
            pk.MetDate = DateTime.Today;

            if (tr.Game >= 0)
                pk.Version = tr.Game;
            int spec = ((GameVersion)pk.Version).GetMaxSpeciesID();
            if (spec <= 0)
                spec = pk.MaxSpeciesID;
            pk.Species = spec;
            var lang = tr.Language;
            if (lang <= 0)
                lang = (int)LanguageID.English;
            pk.Language = lang;

            pk.ClearNickname();

            pk.OT_Name = tr.OT;
            pk.OT_Gender = tr.Gender;
            pk.TID = tr.TID;
            pk.SID = tr.SID;
            if (tr.ConsoleRegion != 0)
            {
                pk.ConsoleRegion = tr.ConsoleRegion;
                pk.Country = tr.Country;
                pk.Region = tr.SubRegion;
            }

            // Copy OT trash bytes for sensitive games (Gen1/2)
            if (tr is SAV1 s1 && pk is PK1 p1) p1.OT_Trash = s1.OT_Trash;
            else if (tr is SAV2 s2 && pk is PK2 p2) p2.OT_Trash = s2.OT_Trash;
        }
    }
}
