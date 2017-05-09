using System;
using System.Collections.Generic;
using System.Linq;

namespace _2labMisoi
{
    public class KMeans
    {
        private List<Region> _regions;
        private Random _random;
        private int _countOfClasses;
        private List<List<Region>> _cores = new List<List<Region>>();
        private HashSet<int> setOfClasses = new HashSet<int>();
        private List<int> indexCores;
        private bool transference = false;
        private List<Region> oldCores = new List<Region>();

        public KMeans(List<Region> regions, int countOfClasses)
        {
            _countOfClasses = countOfClasses;
            _regions = regions;
            _random = new Random();

            for (var i = 0; i < countOfClasses; i++)
                _cores.Add(new List<Region>());

            while (setOfClasses.Count < _countOfClasses)
                setOfClasses.Add(_random.Next(0, _regions.Count));

            indexCores = setOfClasses.ToList();

            //for (var i = 0; i < _countOfClasses; i++)
            //{
            //    _cores[i].Add(_regions[indexCores[i]]);
            //}

            for (var i = 0; i < _regions.Count; i++)
                _regions[i].index = i;
        }

        public List<Region> CalculateAreas()
        {
            List<Region> endRegions = null;

            for (var i = 0; i < indexCores.Count; i++)
            {
                oldCores.Add(_regions[indexCores[i]]);
            }

            int a = 0;
            while (!transference)
            {
                endRegions = Сlusterization(oldCores);
                oldCores =  UpdateCores();
                a++;
            }

            return endRegions;
        }

        public void Some()
        {
            double[,] distances = new double[_regions.Count, _regions.Count];

            for (var i = 0; i < _regions.Count; i++)
            {
                for (var j = 0; j < _regions.Count; j++)
                {
                    distances[i, j] = Math.Sqrt(Math.Pow((_regions[i].Сompactness - _regions[j].Сompactness), 2)
                                                  + Math.Pow(_regions[i].Area - _regions[j].Area, 2)
                                                  + Math.Pow(_regions[i].Perimeter - _regions[j].Perimeter, 2));
                }
            }

            //for (int i = 0; i < _regions.Count; i++)
            //{
            //    for (int j = 0; j < _regions.Count; j++)
            //    {
            //        Debug.Write(distances[i, j] + "|");
            //    }
            //    Debug.WriteLine("-");
            //}
        }

        public List<Region> Сlusterization(List<Region> cores)
        {
            _cores = new List<List<Region>>();
            
            for (var i = 0; i < cores.Count; i++)
            {
                _cores.Add(new List<Region>());
                _cores[i].Add(_regions[cores[i].index]);
            }

            for (var i = 0; i < _regions.Count; i++)
            {
                double standartCompactness = 100000;

                if (indexCores.Contains(i))
                    continue;

                int indexJ = 0;
                for (var j = 0; j < indexCores.Count; j++)
                {
                    double difference = Math.Sqrt(Math.Pow(cores[j].Area - _regions[i].Area, 2));
                        //+ Math.Pow(cores[j].Area - _regions[i].Area, 2) + Math.Pow(cores[j].Perimeter - _regions[i].Perimeter, 2));

                    if (difference < standartCompactness)
                    {
                        standartCompactness = difference;
                        indexJ = j;
                    }
                }

                _cores[indexJ].Add(_regions[i]);
                _regions[i]._color = cores[indexJ]._color;
            }

            return _regions;
        }

        public List<Region> UpdateCores()
        {
            var averageArea = new List<double>();

            for (var i = 0; i < _cores.Count; i++)
            {
                double sumArea = 0;

                for (var j = 0; j < _cores[i].Count; j++)
                {
                    sumArea += _cores[i][j].Area;
                }

                sumArea /= _cores[i].Count;
                averageArea.Add(sumArea);
            }

            var newCores = new List<Region>();

            for (var i = 0; i < _cores.Count; i++)
            {
                double difference = 100000;
                int index = 0;

                for (var j = 0; j < _cores[i].Count; j++)
                {
                    if (Math.Abs(_cores[i][j].Area - averageArea[i]) < difference)
                    {
                        difference = Math.Abs(_cores[i][j].Area - averageArea[i]);
                        index = j;
                    }
                }

                //новые ядра в каждом из классов
                newCores.Add(_cores[i][index]);
            }

            indexCores = new List<int>();
            for (var i = 0; i < newCores.Count; i++)
            {
                indexCores.Add(newCores[i].index);
            }


            bool flag = true;

            for (var i = 0; i < newCores.Count; i++)
            {
                if (newCores[i].index != oldCores[i].index)
                {
                    flag = false;
                }
            }

            if (!flag)
            {
                transference = false;
            }
            else
            {
                transference = true;
            }

            return newCores;
        }
    }
}