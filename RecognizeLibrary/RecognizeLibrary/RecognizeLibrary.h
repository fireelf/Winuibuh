// RecognizeLibrary.h

#pragma once

#include <baseapi.h>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Text;


#pragma comment(lib, "libtesseractd.lib")
#pragma comment(lib, "liblept.lib")

namespace RecognizeLibrary {

	public ref struct TextBlock
	{
	public: int top, left, right, bottom;
			public: String^ Text;
	};

	public ref class Recognator
	{
	private:
		tesseract::TessBaseAPI *api;
		char* String2Char(String^ input)
		{
			char* res = new char[input->Length];
			for(int i= 0; i<input->Length; i++)
			{
				res[i] =  (char)input[i];
			}
			res[input->Length] = '\0';

			return res;
		};
	    String^ DecodeFromUtf8(char* utf8String)
		{
			List<unsigned char>^ tmp = gcnew List<unsigned char>();
			for (int i = 0; utf8String[i] != '\0'; i++)
			{
				tmp->Add(utf8String[i]);
			}
			Encoding^ encoding = Encoding::UTF8;
			return encoding->GetString(tmp->ToArray());
		}
	public:
		Recognator(String^ DataPath, String^ Language)
		{
			api = new tesseract::TessBaseAPI();
			api->Init(String2Char(DataPath), String2Char(Language));
		};
		Recognator()
		{
			api = new tesseract::TessBaseAPI();
			api->Init(NULL, "rus");
		};
		void SetImage(String^ ImagePath)
		{
			char* Path = String2Char(ImagePath);
			PIX   *pixs;
			if ((pixs = pixRead(Path)) == NULL)
				throw gcnew Exception("Не найдено изображение!");
			//pixDestroy(&pixs);

			api->SetImage(pixs);
			api->SetPageSegMode(tesseract::PageSegMode::PSM_AUTO);
		}
		List<TextBlock^>^ GetBlocks()
		{
			List<TextBlock^>^ res = gcnew List<TextBlock^>();
			tesseract::PageIterator *Iterator = api->AnalyseLayout();
			while (Iterator->Next(tesseract::PageIteratorLevel::RIL_WORD))
			{
				TextBlock^ Cur = gcnew TextBlock();
				int top, left, right, bottom;
				Iterator->BoundingBoxInternal(tesseract::PageIteratorLevel::RIL_WORD, &left, &top, &right, &bottom);
				Cur->top = top;
				Cur->left = left;
				Cur->right = right;
				Cur->bottom = bottom;
				res->Add(Cur);

				api->SetRectangle(left - 1, top - 1, right - left + 2, bottom - top + 2);
				char* text = api->GetUTF8Text();

				Cur->Text = DecodeFromUtf8(text);

				
			}
			return res;
		}


	};
}
