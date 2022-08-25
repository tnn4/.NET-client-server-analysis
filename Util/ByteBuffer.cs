namespace Util
{
    public class ByteBuffer
    {
        byte[]  _byteBuffer;
        /// <summary>
        /// create byte array of size n
        /// </summary>
        /// <param name="n"></param>
        ByteBuffer(int n)
        {
            _byteBuffer = new byte[n];
        }
    }

    public struct SByteBufferxy
    {
        public sbyte x
        {
            get;
            set;
        }
        public sbyte y
        {
            get;
            set;
        }
    }
}