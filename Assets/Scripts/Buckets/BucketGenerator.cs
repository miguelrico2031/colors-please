using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BucketGenerator : MonoBehaviour
{
    [Header("Bucket Spawning")]
    [SerializeField] private int _minBuckets;
    [SerializeField] private int _maxBuckets;
    [SerializeField] private RectTransform _bucketUIGroup;
    [SerializeField] private BucketUI _bucketUIPrefab;
    [SerializeField] private List<Bucket> _buckets;
    [SerializeField]private SwipeController _swipeController;
    
    private void Start()
    {
        GenerateBuckets();
    }
    
    public void GenerateBuckets()
    {
        int bucketCount = Random.Range(_minBuckets, _maxBuckets + 1);

        
        _bucketUIGroup.position -= (bucketCount - 1) * (_swipeController.ItemStep / 2f);
        HashSet<Bucket> buckets = new HashSet<Bucket>();
        for (int i = 0; i < bucketCount; i++)
        {
            var bucketUI = Instantiate(_bucketUIPrefab, _bucketUIGroup);

            Bucket randomBucket;
            do randomBucket = _buckets[Random.Range(0, _buckets.Count)];
            while (buckets.Contains(randomBucket));
            buckets.Add(randomBucket);
            bucketUI.Initialize(randomBucket);
        }
        
        _swipeController.Initialize(bucketCount);
    }
}
