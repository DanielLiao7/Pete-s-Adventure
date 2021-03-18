using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class SiblingsTile : RuleTile<SiblingsTile.Neighbor> {
    public List<TileBase> Siblings = new List<TileBase>();

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
            case TilingRuleOutput.Neighbor.This:
                return tile == this || Siblings.Contains(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }
}