namespace System
{
    public static class UlidExtensions
    {
        public static Ulid Inverse(this Ulid ulid)
        {
            var ulidBytes = ulid.ToByteArray();
            byte[] invertedBytes = new byte[16];

            invertedBytes[0] = (byte)~ulidBytes[0];
            invertedBytes[1] = (byte)~ulidBytes[1];
            invertedBytes[2] = (byte)~ulidBytes[2];
            invertedBytes[3] = (byte)~ulidBytes[3];
            invertedBytes[4] = (byte)~ulidBytes[4];
            invertedBytes[5] = (byte)~ulidBytes[5];
            invertedBytes[6] = (byte)~ulidBytes[6];
            invertedBytes[7] = (byte)~ulidBytes[7];
            invertedBytes[8] = (byte)~ulidBytes[8];
            invertedBytes[9] = (byte)~ulidBytes[9];
            invertedBytes[10] = (byte)~ulidBytes[10];
            invertedBytes[11] = (byte)~ulidBytes[11];
            invertedBytes[12] = (byte)~ulidBytes[12];
            invertedBytes[13] = (byte)~ulidBytes[13];
            invertedBytes[14] = (byte)~ulidBytes[14];
            invertedBytes[15] = (byte)~ulidBytes[15];

            return new Ulid(invertedBytes);
        }
    }
}