using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BucketGenerator : MonoBehaviour
{
    [SerializeField] private SwipeController _swipeController;
    [SerializeField] private RectTransform _bucketUIGroup;
    [SerializeField] private BucketUI _bucketUIPrefab;
    
    [SerializeField] private List<Bucket> _powerUpBuckets;
    
    [Serializable]
    public class UniqueBucketsList
    {
        public List<Bucket> Buckets;
    }
    [Header("Se selecciona 1 cubo de cada sublista de esta lista")]
    [SerializeField] private List<UniqueBucketsList> _uniqueBuckets;
    
    [Header("Se selecciona random entre min y max cubos de esta lista")]
    [SerializeField] private List<Bucket> _nonUniqueBuckets;
    [SerializeField] private int _minBucketsFromNone;
    [SerializeField] private int _maxBucketsFromNone;

    private int _bucketCount;

    private void Start()
    {
        GenerateBuckets();
    }

    public void GenerateBuckets()
    {
        _bucketCount = 0;
        
        List<Bucket> buckets = new List<Bucket>();
        foreach (var uniqueBucketList in _uniqueBuckets)
        {
            var bucket = uniqueBucketList.Buckets[Random.Range(0, uniqueBucketList.Buckets.Count)];
            buckets.Add(bucket);
        }

        if (_maxBucketsFromNone > _nonUniqueBuckets.Count)
            throw new Exception("Add more non unique buckets or decrease the max amount.");
        
        int nonUniqueBucketCount = Random.Range(_minBucketsFromNone, _maxBucketsFromNone + 1);
        for (int i = 0; i < nonUniqueBucketCount; i++)
        {
            Bucket randomBucket;
            do randomBucket = _nonUniqueBuckets[Random.Range(0, _nonUniqueBuckets.Count)];
            while (buckets.Contains(randomBucket));
            buckets.Add(randomBucket);
        }
        
        //randomizarla antes de opwerups, asi quedan a la izquierda pero random los cubos de lore y tal
        //y a la derecha los powerups
        buckets.Shuffle();
        
        buckets.AddRange(_powerUpBuckets);
        
        //crear los cubos
        foreach (var bucket in buckets)
            CreateBucketUI(bucket);

        _bucketUIGroup.position -= (_bucketCount - 1) * (_swipeController.ItemStep / 2f);
        _swipeController.Initialize(_bucketCount);
    }


    private void CreateBucketUI(Bucket bucket)
    {
        var bucketUI = Instantiate(_bucketUIPrefab, _bucketUIGroup);
        bucketUI.Initialize(bucket);
        _bucketCount++;
    }
}