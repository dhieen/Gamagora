#pragma once

#include <ppltasks.h>	// Pour create_task

namespace DX
{
	inline void ThrowIfFailed(HRESULT hr)
	{
		if (FAILED(hr))
		{
			// Définir un point d'arrêt sur cette ligne pour intercepter les erreurs d'API Win32.
			throw Platform::Exception::CreateException(hr);
		}
	}

	// Fonction qui lit les données d'un fichier binaire de manière asynchrone.
	inline Concurrency::task<std::vector<byte>> ReadDataAsync(const std::wstring& filename)
	{
		using namespace Windows::Storage;
		using namespace Concurrency;

		auto folder = Windows::ApplicationModel::Package::Current->InstalledLocation;

		return create_task(folder->GetFileAsync(Platform::StringReference(filename.c_str()))).then([](StorageFile^ file)
		{
			return FileIO::ReadBufferAsync(file);
		}).then([](Streams::IBuffer^ fileBuffer) -> std::vector<byte>
		{
			std::vector<byte> returnBuffer;
			returnBuffer.resize(fileBuffer->Length);
			Streams::DataReader::FromBuffer(fileBuffer)->ReadBytes(Platform::ArrayReference<byte>(returnBuffer.data(), fileBuffer->Length));
			return returnBuffer;
		});
	}

	// Convertit une longueur en pixels indépendants du périphérique (DIP) en longueur en pixels physiques.
	inline float ConvertDipsToPixels(float dips, float dpi)
	{
		static const float dipsPerInch = 96.0f;
		return floorf(dips * dpi / dipsPerInch + 0.5f); // Arrondir à l'entier le plus proche.
	}

	// Assignez un nom à l'objet pour faciliter le débogage.
#if defined(_DEBUG)
	inline void SetName(ID3D12Object* pObject, LPCWSTR name)
	{
		pObject->SetName(name);
	}
#else
	inline void SetName(ID3D12Object*, LPCWSTR)
	{
	}
#endif
}

// Fonction d'assistance à l'affectation de noms pour ComPtr<T>.
// Assigne le nom de la variable en tant que nom de l'objet.
#define NAME_D3D12_OBJECT(x) DX::SetName(x.Get(), L#x)
