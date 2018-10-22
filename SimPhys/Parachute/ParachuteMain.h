#pragma once

#include "Common\StepTimer.h"
#include "Common\DeviceResources.h"
#include "Content\Sample3DSceneRenderer.h"

// Affiche le contenu Direct3D à l'écran.
namespace Parachute
{
	class ParachuteMain
	{
	public:
		ParachuteMain();
		void CreateRenderers(const std::shared_ptr<DX::DeviceResources>& deviceResources);
		void Update();
		bool Render();

		void OnWindowSizeChanged();
		void OnSuspending();
		void OnResuming();
		void OnDeviceRemoved();

	private:
		// TODO: remplacez par vos propres convertisseurs de contenu.
		std::unique_ptr<Sample3DSceneRenderer> m_sceneRenderer;

		// Minuteur de boucle de rendu.
		DX::StepTimer m_timer;
	};
}