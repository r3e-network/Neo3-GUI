namespace Neo.Common.Utility
{
    /// <summary>
    /// UTF8 validation utility
    /// </summary>
    public static class Utf8Validator
    {
        private const byte Mask1 = 0b1000_0000;
        private const byte Mask2 = 0b1110_0000;
        private const byte Mask3 = 0b1111_0000;
        private const byte Mask4 = 0b1111_1000;
        private const byte MaskExtra = 0b1100_0000;

        public static bool IsValid(byte[] bytes)
        {
            short extra = 0;
            foreach (byte b in bytes)
            {
                if (extra > 0)
                {
                    extra--;
                    if ((b & MaskExtra) != 0b1000_0000)
                        return false;
                    continue;
                }
                if ((b & Mask1) == 0) continue;
                if ((b & Mask2) == 0b1100_0000) { extra = 1; continue; }
                if ((b & Mask3) == 0b1110_0000) { extra = 2; continue; }
                if ((b & Mask4) == 0b1111_0000) { extra = 3; continue; }
                return false;
            }
            return extra == 0;
        }
    }
}
