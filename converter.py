import fitz  # PyMuPDF
import re

def extract_coordinates_from_pdf(pdf_path):
    figures = []

    # Open the PDF file
    with fitz.open(pdf_path) as doc:
        for page_number, page in enumerate(doc):
            # Extract text from the page
            text = page.get_text()

            # Find coordinates in the text using a regex (example: (x, y))
            coordinates = re.findall(r'\((\d+\.\d*),\s*(\d+\.\d*)\)', text)

            # Convert coordinates to tuples of floats
            points = [(float(x), float(y)) for x, y in coordinates]

            if points:
                figures.append({
                    "page": page_number + 1,
                    "points": points
                })

    return figures

if __name__ == "__main__":
    pdf_path = "path_to_your_drawing.pdf"
    figures = extract_coordinates_from_pdf(pdf_path)

    for figure in figures:
        print(f"Page: {figure['page']}, Points: {figure['points']}")
