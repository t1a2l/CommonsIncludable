using Commons.ModShared;
using Commons.Utils;
using UnityEngine;

namespace Commons.Interfaces
{
    public class BaseController<U, C> : MonoBehaviour
        where U : BasicIUserModSimplified<U, C>, new()
        where C : BaseController<U, C>
    {
        public void Start() => StartActions();

        protected virtual string ClassBridgeUUI { get; } = "Commons.ModShared.BridgeUUI";

        private IBridgeUUI m_bridgeUUI;
        internal IBridgeUUI BridgeUUI
        {
            get
            {
                if (m_bridgeUUI is null)
                {
                    m_bridgeUUI = BasicIUserModSimplified<U, C>.UseUuiIfAvailable
                        ? PluginUtils.GetImplementationTypeForMod<BridgeUUIFallback, IBridgeUUI>(gameObject, "UnifiedUILib", "2.2.9", ClassBridgeUUI)
                        : gameObject.AddComponent<BridgeUUIFallback>();
                }
                return m_bridgeUUI;
            }
        }
        public void FallbackToKButton()
        {
            Destroy(m_bridgeUUI);
            m_bridgeUUI = gameObject.AddComponent<BridgeUUIFallback>();
        }

        protected virtual void StartActions() { }
    }
}