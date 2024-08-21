#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace pddenhar.Planets.Logic;

// Thank you to https://badecho.com/index.php/2023/01/14/fast-simple-quadtree/
// for the excellent explanation of QuadTrees

public partial class QuadTree : Node2D
{
    private readonly List<Node2D> _elements = new();

    private Rect2 _bounds;
    private readonly int _bucketCapacity;
    private readonly int _maxDepth;
 
    private QuadTree? _ul;
    private QuadTree? _ur;
    private QuadTree? _bl;
    private QuadTree? _br;
    
    public int Level { get; set; }
 
    private bool IsLeaf
        => _ul == null || _ur == null || _bl == null || _br == null;
    
    public QuadTree() : this(new Rect2(-100, -100, 200, 200))
    { }
    
    public QuadTree(Rect2 bounds)
        : this(bounds, 32, 5)
    { }
    
    public QuadTree(Rect2 bounds, int bucketCapacity, int maxDepth)
    {
        _bucketCapacity = bucketCapacity;
        _maxDepth = maxDepth;
        _bounds = bounds;
    }
    
    public void Insert(Node2D node)
    {
        if (!_bounds.HasPoint(node.GlobalPosition))
        {
            throw new ArgumentException("Node is outside of quadtree bounds");
        }

        if (_elements.Count >= _bucketCapacity)
            Split();
        
        QuadTree? bucket = GetChildBucket(node.GlobalPosition);
 
        if (bucket != null)
            bucket.Insert(node);
        else
            // this is a leaf node
            _elements.Add(node);
        
    }

    public bool Remove(Node2D element)
    {
        throw new NotImplementedException();
    }

    private QuadTree? GetChildBucket(Vector2 nodeGlobalPosition)
    {
        if (IsLeaf)
            return null;

        if (_ul!._bounds.HasPoint(nodeGlobalPosition))
            return _ul;
        if (_ur!._bounds.HasPoint(nodeGlobalPosition))
            return _ur;
        if (_bl!._bounds.HasPoint(nodeGlobalPosition))
            return _bl;
        return _br!._bounds.HasPoint(nodeGlobalPosition) ? _br : null;

    }

    private void Split()
    {
        if (!IsLeaf)
            return;
        
        if (Level + 1 > _maxDepth)
            return;

        var boundsCenter = _bounds.GetCenter();
        
        _ul = CreateChild(_bounds.Position);
        _ur = CreateChild(new Vector2(boundsCenter.X, _bounds.Position.Y));
        _bl = CreateChild(new Vector2(_bounds.Position.X, boundsCenter.Y));
        _br = CreateChild(boundsCenter);

        // Shallow copy list so we can modify it inside the for loop
        var elements = _elements.ToList();

        foreach (var element in elements)
        {
            var containingChild = GetChildBucket(element.GlobalPosition);
            // An element is only moved if it completely fits into a child quadrant.
            if (containingChild != null)
            {
                _elements.Remove(element);
                containingChild.Insert(element);
            }
        }
    }

    private QuadTree CreateChild(Vector2 location)
        => new(new Rect2(location, _bounds.Size/2), _bucketCapacity, _maxDepth)
        {
            Level = Level + 1
        };
    
    private void Merge()
    {   // If we're a leaf node, then there is nothing to merge.
        if (IsLeaf)
            return;
        // disable null checks since IsLeaf performs them
        _elements.AddRange(_ul!._elements);
        _elements.AddRange(_ur!._elements);
        _elements.AddRange(_bl!._elements);
        _elements.AddRange(_br!._elements);

        _ul = _ur = _bl = _br = null;
    }

}