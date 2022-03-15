using System;

namespace MAI2.Memory
{
	public class FixedAllocator
	{
		private int chunkSize_;

		private int capacity_;

		private int size_;

		private Chunk[] chunks_;

		private byte[] buffer_;

		public int ChunkSize => chunkSize_;

		public int Size => size_;

		public int Capacity => capacity_;

		public FixedAllocator(int chunkSize, int maxChunks)
		{
			chunkSize_ = chunkSize;
			expand(maxChunks);
		}

		public Chunk allocate()
		{
			if (size_ <= 0)
			{
				return new Chunk(0, 0, null);
			}
			size_--;
			return chunks_[size_];
		}

		public void free(ref Chunk chunk)
		{
			if (!chunk.IsNull)
			{
				chunks_[size_] = chunk;
				size_++;
				chunk = new Chunk(0, 0, null);
			}
		}

		private void expand(int newCapacity)
		{
			chunks_ = new Chunk[newCapacity];
			Array.Resize(ref buffer_, chunkSize_ * newCapacity);
			size_ = newCapacity - capacity_;
			int num = chunkSize_ * capacity_;
			for (int i = 0; i < size_; i++)
			{
				chunks_[i] = new Chunk(num, chunkSize_, buffer_);
				num += chunkSize_;
			}
			capacity_ = newCapacity;
		}
	}
}
