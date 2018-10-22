#include "pch.h"
#include "App.h"

#include <ppltasks.h>

using namespace Parachute;

using namespace concurrency;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

using Microsoft::WRL::ComPtr;

// Pour plus d'informations sur le mod�le Application DirectX�12, consultez la page https://go.microsoft.com/fwlink/?LinkID=613670&clcid=0x409

// La fonction principale est utilis�e uniquement pour initialiser notre classe IFrameworkView.
[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
	auto direct3DApplicationSource = ref new Direct3DApplicationSource();
	CoreApplication::Run(direct3DApplicationSource);
	return 0;
}

IFrameworkView^ Direct3DApplicationSource::CreateView()
{
	return ref new App();
}

App::App() :
	m_windowClosed(false),
	m_windowVisible(true)
{
}

// Premi�re m�thode appel�e durant la cr�ation de IFrameworkView.
void App::Initialize(CoreApplicationView^ applicationView)
{
	// Inscrivez les gestionnaires d'�v�nements pour le cycle de vie de l'application. Cet exemple inclut Activ�, pour que nous
	// peut activer CoreWindow et d�marrer le rendu sur la fen�tre.
	applicationView->Activated +=
		ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &App::OnActivated);

	CoreApplication::Suspending +=
		ref new EventHandler<SuspendingEventArgs^>(this, &App::OnSuspending);

	CoreApplication::Resuming +=
		ref new EventHandler<Platform::Object^>(this, &App::OnResuming);
}

// Appel� quand l'objet CoreWindow est cr�� (ou recr��).
void App::SetWindow(CoreWindow^ window)
{
	window->SizeChanged += 
		ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &App::OnWindowSizeChanged);

	window->VisibilityChanged +=
		ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &App::OnVisibilityChanged);

	window->Closed += 
		ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &App::OnWindowClosed);

	DisplayInformation^ currentDisplayInformation = DisplayInformation::GetForCurrentView();

	currentDisplayInformation->DpiChanged +=
		ref new TypedEventHandler<DisplayInformation^, Object^>(this, &App::OnDpiChanged);

	currentDisplayInformation->OrientationChanged +=
		ref new TypedEventHandler<DisplayInformation^, Object^>(this, &App::OnOrientationChanged);

	DisplayInformation::DisplayContentsInvalidated +=
		ref new TypedEventHandler<DisplayInformation^, Object^>(this, &App::OnDisplayContentsInvalidated);
}

// Initialise les ressources de sc�ne ou charge un �tat d'application pr�c�demment enregistr�.
void App::Load(Platform::String^ entryPoint)
{
	if (m_main == nullptr)
	{
		m_main = std::unique_ptr<ParachuteMain>(new ParachuteMain());
	}
}

// Cette m�thode est appel�e une fois que la fen�tre est active.
void App::Run()
{
	while (!m_windowClosed)
	{
		if (m_windowVisible)
		{
			CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);

			auto commandQueue = GetDeviceResources()->GetCommandQueue();
			PIXBeginEvent(commandQueue, 0, L"Update");
			{
				m_main->Update();
			}
			PIXEndEvent(commandQueue);

			PIXBeginEvent(commandQueue, 0, L"Render");
			{
				if (m_main->Render())
				{
					GetDeviceResources()->Present();
				}
			}
			PIXEndEvent(commandQueue);
		}
		else
		{
			CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
		}
	}
}

// N�cessaire pour IFrameworkView.
// Les �v�nements Terminate n'entra�nent pas l'appel de Uninitialize. Il sera appel� si votre classe IFrameworkView
// est d�truite quand l'application est au premier plan.
void App::Uninitialize()
{
}

// Gestionnaires d'�v�nements du cycle de vie de l'application.

void App::OnActivated(CoreApplicationView^ applicationView, IActivatedEventArgs^ args)
{
	// Run() ne d�marrera pas tant que CoreWindow n'aura pas �t� activ�.
	CoreWindow::GetForCurrentThread()->Activate();
}

void App::OnSuspending(Platform::Object^ sender, SuspendingEventArgs^ args)
{
	// Enregistrer l'�tat de l'application de mani�re asynchrone apr�s avoir demand� un report. Un report en attente
	// indique que l'application ex�cute des op�rations de suspension. Noter
	// qu'un report ne peut pas rester en attente ind�finiment. Apr�s environ cinq secondes,
	// l'application devra se fermer.
	SuspendingDeferral^ deferral = args->SuspendingOperation->GetDeferral();

	create_task([this, deferral]()
	{
		m_main->OnSuspending();
		deferral->Complete();
	});
}

void App::OnResuming(Platform::Object^ sender, Platform::Object^ args)
{
	// Restaurer les donn�es ou l'�tat d�charg�s durant la suspension. Par d�faut, les donn�es
	// et l'�tat sont persistants durant la reprise apr�s la suspension. Noter que cet �v�nement
	// ne se produit pas si l'application a �t� arr�t�e pr�c�demment.

	m_main->OnResuming();
}

// Gestionnaires d'�v�nements de fen�tre.

void App::OnWindowSizeChanged(CoreWindow^ sender, WindowSizeChangedEventArgs^ args)
{
	GetDeviceResources()->SetLogicalSize(Size(sender->Bounds.Width, sender->Bounds.Height));
	m_main->OnWindowSizeChanged();
}

void App::OnVisibilityChanged(CoreWindow^ sender, VisibilityChangedEventArgs^ args)
{
	m_windowVisible = args->Visible;
}

void App::OnWindowClosed(CoreWindow^ sender, CoreWindowEventArgs^ args)
{
	m_windowClosed = true;
}

// Gestionnaires d'�v�nements DisplayInformation.

void App::OnDpiChanged(DisplayInformation^ sender, Object^ args)
{
	// Remarque�: la valeur de LogicalDpi r�cup�r�e ici peut ne pas correspondre � la r�solution effective de l'application
	// si elle est mise � l'�chelle pour les appareils haute r�solution. Une fois la valeur PPP d�finie sur DeviceResources,
	// vous devez toujours la r�cup�rer � l'aide de la m�thode GetDpi.
	// Consultez DeviceResources.cpp pour plus d'informations.
	GetDeviceResources()->SetDpi(sender->LogicalDpi);
	m_main->OnWindowSizeChanged();
}

void App::OnOrientationChanged(DisplayInformation^ sender, Object^ args)
{
	GetDeviceResources()->SetCurrentOrientation(sender->CurrentOrientation);
	m_main->OnWindowSizeChanged();
}

void App::OnDisplayContentsInvalidated(DisplayInformation^ sender, Object^ args)
{
	GetDeviceResources()->ValidateDevice();
}

std::shared_ptr<DX::DeviceResources> App::GetDeviceResources()
{
	if (m_deviceResources != nullptr && m_deviceResources->IsDeviceRemoved())
	{
		// Toutes les r�f�rences � l'appareil D3D existant doivent �tre lib�r�es pour permettre
		// la cr�ation d'un appareil.

		m_deviceResources = nullptr;
		m_main->OnDeviceRemoved();

#if defined(_DEBUG)
		ComPtr<IDXGIDebug1> dxgiDebug;
		if (SUCCEEDED(DXGIGetDebugInterface1(0, IID_PPV_ARGS(&dxgiDebug))))
		{
			dxgiDebug->ReportLiveObjects(DXGI_DEBUG_ALL, DXGI_DEBUG_RLO_FLAGS(DXGI_DEBUG_RLO_SUMMARY | DXGI_DEBUG_RLO_IGNORE_INTERNAL));
		}
#endif
	}

	if (m_deviceResources == nullptr)
	{
		m_deviceResources = std::make_shared<DX::DeviceResources>();
		m_deviceResources->SetWindow(CoreWindow::GetForCurrentThread());
		m_main->CreateRenderers(m_deviceResources);
	}
	return m_deviceResources;
}
