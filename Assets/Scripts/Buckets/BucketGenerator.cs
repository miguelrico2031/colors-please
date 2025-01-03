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
    [SerializeField] private BucketUI _piggyBankBucketUIPrefab;
    
    [Serializable]
    public class UniqueBucketsList
    {
        public Character Character;
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
        HashSet<Character> dayBucketsCharacter = new();
        var day = ServiceLocator.Get<IDayService>().CurrentDay;
        buckets.AddRange(day.Buckets.Select(characterBucket =>
        {
            dayBucketsCharacter.Add(characterBucket.Character);
            return characterBucket.Bucket;
        }));
        
        foreach (var uniqueBucketList in _uniqueBuckets)
        {
            if(dayBucketsCharacter.Contains(uniqueBucketList.Character)) continue;
            var bucket = uniqueBucketList.Buckets[Random.Range(0, uniqueBucketList.Buckets.Count)];
            buckets.Add(bucket);
        }

        if (_maxBucketsFromNone > _nonUniqueBuckets.Count)
            throw new Exception("Add more non unique buckets or decrease the max amount.");
        
        int nonUniqueBucketCount = day.SkipNonUniqueBuckets ? 0 :
            Random.Range(_minBucketsFromNone, _maxBucketsFromNone + 1);
        
        for (int i = 0; i < nonUniqueBucketCount; i++)
        {
            Bucket randomBucket;
            do randomBucket = _nonUniqueBuckets[Random.Range(0, _nonUniqueBuckets.Count)];
            while (buckets.Contains(randomBucket));
            buckets.Add(randomBucket);
        }

        buckets.Shuffle();
        
        //crear los cubos
        foreach (var bucket in buckets)
            CreateBucketUI(bucket, _bucketUIPrefab);
        
        CreateBucketUI(null, _piggyBankBucketUIPrefab);

        _bucketUIGroup.position -= (_bucketCount - 1) * (_swipeController.ItemStep / 2f);
        _swipeController.Initialize(_bucketCount);
    }


    private void CreateBucketUI(Bucket bucket, BucketUI prefab)
    {
        var bucketUI = Instantiate(prefab, _bucketUIGroup);
        bucketUI.Initialize(bucket);
        _bucketCount++;
    }
}