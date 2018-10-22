#include "pch.h"
#include "ParachuteMain.h"
#include "Common\DirectXHelper.h"

using namespace Parachute;
using namespace Windows::Foundation;
using namespace Windows::System::Threading;
using namespace Concurrency;

// Pour plus d'informations sur le modèle Application DirectX 12, consultez la page https://go.microsoft.com/fwlink/?LinkID=613670&clcid=0x409

// Charge et initialise les actifs de l'application durant son chargement.
ParachuteMain::ParachuteMain()
{
	// TODO: modifiez les paramètres de minuterie si vous voulez utiliser autre chose que le mode timestep variable par défaut.
	// par exemple, pour une logique de mise à jour timestep fixe de 60 FPS, appelez :
	/*
	m_timer.SetFixedTimeStep(true) ;
	m_timer.SetTargetElapsedSeconds(1.0 / 60) ;
	*/
}

// Crée et initialise les convertisseurs.
void ParachuteMain::CreateRenderers(const std::shared_ptr<DX::DeviceResources>& deviceResources)
{
	// TODO: remplacez ceci par l'initialisation du contenu de votre application.
	m_sceneRenderer = std::unique_ptr<Sample3DSceneRenderer>(new Sample3DSceneRenderer(deviceResources));

	OnWindowSizeChanged();
}

// Met à jour l'état de l'application une fois par frame.
void ParachuteMain::Update()
{
	// Mettre à jour les objets de scène.
	m_timer.Tick([&]()
	{
		// TODO: remplacez ceci par vos fonctions de mise à jour de contenu d'application.
		m_sceneRenderer->Update(m_timer);
	});
}

// Affiche le frame actuel en fonction de l'état actuel de l'application.
// Retourne true si le frame a été rendu et est prêt à être affiché.
bool ParachuteMain::Render()
{
	// N'essayez d'afficher aucun élément avant la première mise à jour.
	if (m_timer.GetFrameCount() == 0)
	{
		return false;
	}

	// Afficher les objets de scène.
	// TODO: remplacez ceci par vos fonctions de rendu de contenu d'application.
	return m_sceneRenderer->Render();
}

// Met à jour l'état de l'application quand la taille de la fenêtre change (par exemple, durant le changement d'orientation de l'appareil)
void ParachuteMain::OnWindowSizeChanged()
{
	// TODO: remplacez ceci par l'initialisation du contenu de votre application en fonction de la taille.
	m_sceneRenderer->CreateWindowSizeDependentResources();
}

// Indique à l'application qu'elle est suspendue.
void ParachuteMain::OnSuspending()
{
	// TODO: remplacez ceci par la logique de suspension de votre application.

	// La gestion de la durée de vie des processus peut terminer les applications suspendues à tout moment. Il est donc
	// conseillé d'enregistrer les états qui permettent à l'application de redémarrer là où elle s'est arrêtée.

	m_sceneRenderer->SaveState();

	// Si votre application utilise les allocations de mémoire vidéo, qui sont faciles à recréer,
	// libérez la mémoire pour la mettre à disposition des autres applications.
}

// Indique à l'application qu'elle n'est plus suspendue.
void ParachuteMain::OnResuming()
{
	// TODO: remplacez ceci par la logique de reprise de votre application.
}

// Avertit les convertisseurs que les ressources du périphérique doivent être libérées.
void ParachuteMain::OnDeviceRemoved()
{
	// TODO: enregistrez l'état nécessaire de l'application ou du convertisseur, puis libérez le convertisseur
	// et ses ressources qui ne sont plus valides.
	m_sceneRenderer->SaveState();
	m_sceneRenderer = nullptr;
}
