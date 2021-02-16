using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Minefield.Persistence
{
    
    public class MinefieldFileDataAccess : IMinefieldDataAccess
    {
        /// <summary>
        /// Fájl betöltése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <returns>A fájlból beolvasott játéktábla.</returns>
        public async Task<ReturnData> LoadAsync(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path)) 
                {
                    ReturnData returndata = new ReturnData();
                    returndata.gameTime = int.Parse(reader.ReadLine());

                    returndata.table = new MinefieldTable();

                    String line;
                    string[] numbers=new string[10];
                    int temp;
                   
                    for (int i = 0; i < 10; i++)
                    {
                        line = await reader.ReadLineAsync();
                        numbers = line.Split(' ');

                        for (int j = 0; j < 10; j++)
                        {
                            temp = int.Parse(numbers[j]);
                            if(temp == 0)
                            {
                                returndata.table.fieldValues[i, j] = FieldType.Empty;
                            }
                            else if(temp == 1)
                            {
                                returndata.table.fieldValues[i, j] = FieldType.LightB;
                            }
                            else if (temp == 2)
                            {
                                returndata.table.fieldValues[i, j] = FieldType.MediumB;
                            }
                            else if (temp == 3)
                            {
                                returndata.table.fieldValues[i, j] = FieldType.HeavyB;
                            }
                            else if (temp == 4)
                            {
                                returndata.table.fieldValues[i, j] = FieldType.Player;
                            }

                        }
                    }
                    
                    return returndata;
                }
            }
            catch
            {
                throw new Exception("Failedload");
            }
        }

        /// <summary>
        /// Fájl mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <param name="table">A fájlba kiírandó játéktábla.</param>
        public async Task SaveAsync(String path, MinefieldTable table, int GameTime)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path)) // fájl megnyitása
                {
                    
                    await writer.WriteLineAsync(GameTime.ToString());
                    int temp=0;

                    for (int i = 0; i < 10 ; i++)
                    {
                        for (int j = 0; j < 10 ; j++)
                        {
                            switch (table.fieldValues[i, j])
                            {
                                case FieldType.Empty:
                                    temp = 0;
                                    break;
                                case FieldType.LightB:
                                    temp = 1;
                                    break;
                                case FieldType.MediumB:
                                    temp = 2;
                                    break;
                                case FieldType.HeavyB:
                                    temp = 3;
                                    break;
                                case FieldType.Player:
                                    temp = 4;
                                    break;
                            }
                            await writer.WriteAsync(temp + " ");
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new Exception("Save failed");
            }
        }
    }
}
