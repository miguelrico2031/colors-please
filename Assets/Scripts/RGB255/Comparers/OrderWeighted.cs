using System.Collections.Generic;
using UnityEngine;

namespace ColorComparer
{
    public class OrderWeighted<C> : IColorComparer where C : IColorComparer, new()
    {
        private C _colorComparer;
        private readonly float _orderWeight;
        
        public float MAX_DISTANCE => Distance(new RGB255(), new RGB255(255, 255, 255));

        public OrderWeighted(float orderWeight)
        {
            _colorComparer = new C();
            _orderWeight = Mathf.Clamp01(orderWeight);
        }
        
        public float Distance(RGB255 a, RGB255 b)
        {
            return _colorComparer.Distance(a, b);
        }

        public float Similarity(RGB255 a, RGB255 b)
        {
            float comparerSimilarity = _colorComparer.Similarity(a, b);
            float orderSimilarity = GetOrderSimilarity(a, b);
            return (1 - _orderWeight) * comparerSimilarity + _orderWeight * orderSimilarity;
        }

        private static float GetOrderSimilarity(RGB255 a, RGB255 b)
        {
            bool completeOrder = true;
            bool completeDisorder = true;
            var aTable = new[] { a.R, a.G, a.B };
            var bTable = new[] { b.R, b.G, b.B };
            
            //listas de indices de las tablas, para ordenarlas segun las tablas
            var aIndexList = new List<int>(new[]{0, 1, 2});
            var bIndexList = new List<int>(new[]{0, 1, 2});
            //ordenar las listas
            aIndexList.Sort((index1, index2) => aTable[index1].CompareTo(aTable[index2]));
            bIndexList.Sort((index1, index2) => bTable[index1].CompareTo(bTable[index2]));
            //comprobar si estan ordenadas bien
            for (int i = 0; i < 3; i++)
            {
                if(aIndexList[i] == bIndexList[i]) completeDisorder = false;
                else completeOrder = false;
            }

            if (completeOrder) return 1f;
            else if (completeDisorder) return 0f;
            else return .5f;
        }
    }
}