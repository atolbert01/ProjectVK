namespace ProjectVK
{
    class ArrayList<T>
    {
        public T[] Items;
        public int Count
        {
            get { return Items.GetUpperBound(0) + 1; }
        }
        public ArrayList()
        {
            Items = new T[0];
        }

        public ArrayList(T[] items)
        {
            Items = items;
        }

        public void Add(T newItem)
        {
            T[] newItems = new T[Items.Length + 1];
            for (int i = 0; i < Items.Length; i++)
            {
                newItems[i] = Items[i];
            }
            newItems[Items.Length] = newItem;
            Items = newItems;
        }

        public void Remove(int removedIndex)
        {
            if (Items.Length != 0)
            {
                T[] resizedItems = new T[Items.Length - 1];

                int j = 0;
                for (int i = 0; i < Items.Length; i++)
                {
                    if (i != removedIndex)
                    {
                        resizedItems[j] = Items[i];
                        j++;
                    }
                }
                Items = resizedItems;
            }
        }

        public void Insert(int currentIndex, T newItem)
        {
            T[] resizedItems = new T[Items.Length + 1];

            int j = 0;
            for (int i = 0; i < resizedItems.Length; i++)
            {
                if (i != currentIndex + 1)
                {
                    resizedItems[i] = Items[j];
                    j++;
                }
                else
                {
                    resizedItems[i] = newItem;
                }
            }
            Items = resizedItems;
        }

        /// <summary>
        /// Reassigns the first index to the item that is passed to it. Shifts all other items up to accommodate.
        /// </summary>
        /// <param name="newHead"></param>
        public void ChangeOrder(int oldIndex, T newHead)
        {
            if (Items.Length > 1)
            {
                T[] shiftedItems = new T[Items.Length];


                shiftedItems[0] = newHead;
                int j = 0;
                for (int i = 1; i < Items.Length; i++)
                {
                    if (i != oldIndex)
                    {
                        shiftedItems[i] = Items[j];
                        j++;
                    }
                    else
                    {
                        shiftedItems[i] = Items[j];
                        // if there are other items in the array this will ensure that we skip newHead in our Items array since we already added it before the loop.
                        // if we are at the end of the array then our j value doesn't matter
                        j += 2;

                    }
                }
            }
        }
    }
}
