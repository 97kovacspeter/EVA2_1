using System;


namespace Minefield.Persistence
{
    #region enum

    public enum FieldType { Empty, LightB, MediumB, HeavyB, Player }

    #endregion

    /// <summary>
    /// Minefield játéktábla típusa.
    /// </summary>
    public class MinefieldTable
    {
        #region Fields

        public FieldType[,] fieldValues; //mezőértékek

        #endregion

        #region Constructor

        public MinefieldTable()
        {
            fieldValues = new FieldType[10,10];
            for(int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    fieldValues[i, j] = FieldType.Empty;
                }
            }
            fieldValues[9, 9] =FieldType.Player;

        }

        #endregion

    }
}
