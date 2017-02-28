using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace DiscordCardBot.Cards_Stuff
{
    class SQLCommands
    {
        public static class SQLiteCommands
        {
            public static List<string[]> LoadData(SqliteConnection connection)
            {
                SqliteCommand datacommand = new SqliteCommand("SELECT id, ot, alias, setcode, type, level, race, attribute, atk, def, category FROM datas", connection);
                return DatabaseHelper.ExecuteStringCommand(datacommand, 11);
            }

            public static List<string[]> LoadText(SqliteConnection connection)
            {
                SqliteCommand textcommand = new SqliteCommand("SELECT id, name, desc, str1, str2, str3, str4, str5, str6, str7, str8, str9, str10, str11, str12, str13, str14, str15, str16 FROM texts", connection);
                return DatabaseHelper.ExecuteStringCommand(textcommand, 19);
            }

            public static bool UpdateCardId(string cardId, string updatedId, SqliteConnection connection)
            {
                try
                {
                    SqliteCommand command = DatabaseHelper.CreateCommand("UPDATE datas SET id=@updatedId WHERE id=@cardId", connection);

                    command.Parameters.Add(new SqliteParameter("@updatedId", updatedId));
                    command.Parameters.Add(new SqliteParameter("@cardId", cardId));
                    DatabaseHelper.ExecuteNonCommand(command);

                    command = DatabaseHelper.CreateCommand("UPDATE texts SET id=@updatedId WHERE id=@cardId", connection);
                    command.Parameters.Add(new SqliteParameter("@updatedId", updatedId));
                    command.Parameters.Add(new SqliteParameter("@cardId", cardId));
                    DatabaseHelper.ExecuteNonCommand(command);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Database ERROR]: {0}", ex.Message);
                    return false;
                }
            }

            public static bool UpdateCardOt(string cardId, string updatedOt, SqliteConnection connection)
            {
                try
                {
                    SqliteCommand command = DatabaseHelper.CreateCommand("UPDATE datas SET ot=@updatedOt WHERE id=@cardId", connection);
                    command.Parameters.Add(new SqliteParameter("@updatedOt", updatedOt));
                    command.Parameters.Add(new SqliteParameter("@cardId", cardId));

                    return DatabaseHelper.ExecuteNonCommand(command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Database ERROR]: {0}", ex.Message);
                    return false;
                }
            }

            public static bool SaveCard(CardInfos card, SqliteConnection connection, int loadedid, bool overwrite)
            {
                try
                {
                    SqliteCommand command;
                    if (overwrite)
                    {
                        command = DatabaseHelper.CreateCommand("UPDATE datas" +
                      " SET id= @id, ot = @ot, alias= @alias, setcode= @setcode, type= @type, atk= @atk, def= @def, level= @level, race= @race, attribute= @attribute, category= @category WHERE id = @loadedid", connection);
                    }
                    else
                    {
                        command = DatabaseHelper.CreateCommand("INSERT INTO datas (id,ot,alias,setcode,type,atk,def,level,race,attribute,category)" +
                            " VALUES (@id, @ot, @alias, @setcode, @type, @atk, @def, @level, @race, @attribute, @category)", connection);
                    }

                    command.Parameters.Add(new SqliteParameter("@loadedid", loadedid));
                    command.Parameters.Add(new SqliteParameter("@id", card.Id));
                    command.Parameters.Add(new SqliteParameter("@ot", card.Ot));
                    command.Parameters.Add(new SqliteParameter("@alias", card.AliasId));
                    command.Parameters.Add(new SqliteParameter("@setcode", card.SetCode));
                    command.Parameters.Add(new SqliteParameter("@type", card.Type));
                    command.Parameters.Add(new SqliteParameter("@atk", card.Atk));
                    command.Parameters.Add(new SqliteParameter("@def", card.Def));
                    command.Parameters.Add(new SqliteParameter("@level", card.GetLevelCode()));
                    command.Parameters.Add(new SqliteParameter("@race", card.Race));
                    command.Parameters.Add(new SqliteParameter("@attribute", card.Attribute));
                    command.Parameters.Add(new SqliteParameter("@category", card.Category));
                    DatabaseHelper.ExecuteNonCommand(command);

                    if (overwrite)
                    {
                        command = DatabaseHelper.CreateCommand("UPDATE texts" +
                        " SET id= @id,name= @name,desc= @des,str1= @str1,str2= @str2,str3= @str3,str4= @str4,str5= @str5,str6= @str6,str7= @str7,str8= @str8,str9= @str9,str10= @str10,str11= @str11,str12= @str12,str13= @str13,str14= @str14,str15= @str15,str16= @str16 WHERE id= @loadedid", connection);
                    }
                    else
                    {
                        command = DatabaseHelper.CreateCommand("INSERT INTO texts (id,name,desc,str1,str2,str3,str4,str5,str6,str7,str8,str9,str10,str11,str12,str13,str14,str15,str16)" +
                        " VALUES (@id,@name,@des,@str1,@str2,@str3,@str4,@str5,@str6,@str7,@str8,@str9,@str10,@str11,@str12,@str13,@str14,@str15,@str16)", connection);
                    }
                    command.Parameters.Add(new SqliteParameter("@loadedid", loadedid));
                    command.Parameters.Add(new SqliteParameter("@id", card.Id));
                    command.Parameters.Add(new SqliteParameter("@name", card.Name));
                    command.Parameters.Add(new SqliteParameter("@des", card.Description));
                    var parameters = new List<SqliteParameter>();
                    for (int i = 0; i < 16; i++)
                    {
                        parameters.Add(new SqliteParameter("@str" + (i + 1), (i < card.EffectStrings.Length ? card.EffectStrings[i].ToString() : string.Empty)));
                    }
                    command.Parameters.AddRange(parameters.ToArray());
                    return DatabaseHelper.ExecuteNonCommand(command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Database ERROR]: {0}", ex.Message);
                    return false;
                }
            }

            public static bool DeleteCard(int cardid, SqliteConnection connection)
            {
                try
                {
                    SqliteCommand command = DatabaseHelper.CreateCommand("DELETE FROM datas WHERE id= @id", connection);
                    command.Parameters.Add(new SqliteParameter("@id", cardid));
                    DatabaseHelper.ExecuteIntCommand(command);
                    command = DatabaseHelper.CreateCommand("DELETE FROM texts WHERE id= @id", connection);
                    command.Parameters.Add(new SqliteParameter("@id", cardid));

                    return DatabaseHelper.ExecuteNonCommand(command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Database ERROR]: {0}", ex.Message);
                    return false;
                }
            }

            public static bool ContainsCard(int cardid, SqliteConnection connection)
            {
                try
                {
                    SqliteCommand checkcommand = DatabaseHelper.CreateCommand("SELECT COUNT(*) FROM datas WHERE id= @id", connection);
                    checkcommand.Parameters.Add(new SqliteParameter("@id", cardid));
                    return DatabaseHelper.ExecuteIntCommand(checkcommand) > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Database ERROR]: {0}", ex.Message);
                    return false;
                }
            }
        }
    }
}
